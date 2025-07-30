using FluentValidation;
using GenericRepository;
using RentCarServer.Application.Services;
using RentCarServer.Domain.Users;
using TS.MediatR;
using TS.Result;

namespace RentCarServer.Application.Auth;
public sealed record LoginCommand(
    string EmailOrUsername,
    string Password) : IRequest<Result<LogincommandResponse>>;

public sealed record LogincommandResponse()
{
    public string? Token { get; init; }
    public string? TFACode { get; init; }
}

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.EmailOrUsername)
            .NotEmpty().WithMessage("Kullanıcı adı ya da e-posta boş olamaz");
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifre boş olamaz");
    }
}
public sealed class LoginCommandHandler(
    IUserRepository userRepository,
    IJwtProvider jwtProvider,
    IMailService mailService,
    IUnitOfWork unitOfWork) : IRequestHandler<LoginCommand, Result<LogincommandResponse>>
{
    public async Task<Result<LogincommandResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.FirstOrDefaultAsync(
            x => x.UserName.Value == request.EmailOrUsername || x.Email.Value == request.EmailOrUsername);

        if (user is null)
        {
            return Result<LogincommandResponse>.Failure("Kullanıcı adı ya da şifre yanlış");
        }

        if (!user.VerifyPasswordHash(request.Password))
        {
            return Result<LogincommandResponse>.Failure("Kullanıcı adı ya da şifre yanlış");
        }

        if (!user.TFAStatus.Value)
        {
            var token = await jwtProvider.CreateTokenAsync(user, cancellationToken);

            var res = new LogincommandResponse
            {
                Token = token
            };
            return res;
        }
        else
        {
            user.CreateTFACode();

            userRepository.Update(user);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            string to = user.Email.Value;
            string subject = "İki Faktörlü Kimlik Doğrulama Kodu";
            string body = $"Merhaba {user.FullName.Value},\n\n" +
                          $"İki faktörlü kimlik doğrulama kodunuz: <h2>{user.TFAConfirmCode!.Value}</h2>\n\n" +
                          "Kod sadece 5 dakika geçerlidir.Bu kodu kullanarak giriş yapabilirsiniz.\n\n" +
                          "Saygılarımızla,\nRentCarServer";
            await mailService.SendAsync(to, subject, body, cancellationToken);

            var res = new LogincommandResponse
            {
                TFACode = user.TFACode!.Value
            };
            return res;
        }

    }
}

