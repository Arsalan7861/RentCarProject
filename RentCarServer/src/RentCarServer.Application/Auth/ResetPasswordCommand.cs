using FluentValidation;
using GenericRepository;
using Microsoft.EntityFrameworkCore;
using RentCarServer.Domain.LoginTokens;
using RentCarServer.Domain.Shared;
using RentCarServer.Domain.Users;
using TS.MediatR;
using TS.Result;

namespace RentCarServer.Application.Auth;
public sealed record ResetPasswordCommand(
    Guid ForgotPasswordCode,
    string NewPassword,
    bool LogoutAllDevices) : IRequest<Result<string>>;

public sealed class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(p => p.NewPassword).NotEmpty().WithMessage("Geçerli bir yeni şifre girin");
    }
}

public sealed record ResetPasswordCommandHandler(
    ILoginTokenRepository loginTokenRepository,
    IUserRepository UserRepository,
    IUnitOfWork UnitOfWork) : IRequestHandler<ResetPasswordCommand, Result<string>>
{
    public async Task<Result<string>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await UserRepository.FirstOrDefaultAsync(p =>
        p.ForgotPasswordCode != null &&
        p.ForgotPasswordCode.Value == request.ForgotPasswordCode
        && p.IsForgotPasswordCompleted.Value == false
        , cancellationToken);

        if (user == null)
        {
            return Result<string>.Failure("Şifre sıfırlama değeriniz geçersiz");
        }

        var fpDate = user.ForgotPasswordDate!.Value.AddDays(1);
        var now = DateTimeOffset.Now;

        if (fpDate < now)
        {
            return Result<string>.Failure("Şifre sıfırlama süresi dolmuş");
        }

        Password password = new(request.NewPassword);
        user.SetPassword(password);
        UserRepository.Update(user);

        if (request.LogoutAllDevices)
        {
            // eski tokenleri pasif hale getir
            var loginTokens = await loginTokenRepository
                .Where(x => x.UserId == user.Id && x.IsActive.Value == true)
                .ToListAsync(cancellationToken);
            foreach (var existingToken in loginTokens)
            {
                existingToken.SetIsActive(new(false));
            }
            loginTokenRepository.UpdateRange(loginTokens);
        }
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return "Şifre başarıyla sıfırlandı, Yeni şifrenizle giriş yapabilirsiniz.";
    }
}
