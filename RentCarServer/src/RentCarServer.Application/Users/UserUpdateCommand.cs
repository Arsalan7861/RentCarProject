using FluentValidation;
using GenericRepository;
using RentCarServer.Application.Behaviors;
using RentCarServer.Application.Services;
using RentCarServer.Domain.Abstractions;
using RentCarServer.Domain.Shared;
using RentCarServer.Domain.Users;
using RentCarServer.Domain.Users.ValueObjects;
using TS.MediatR;
using TS.Result;

namespace RentCarServer.Application.Users;
[Permission("user:edit")]
public sealed record UserUpdateCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string UserName,
    Guid? BranchId,
    Guid RoleId,
    bool IsActive) : IRequest<Result<string>>;

public sealed class UserUpdateCommandValidator : AbstractValidator<UserUpdateCommand>
{
    public UserUpdateCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Geçerli bir ad girin.");
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Geçerli bir soyad girin.");
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Geçerli bir e-posta girin.")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi girin.");
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Geçerli bir kullanıcı adı girin.")
            .MinimumLength(3).WithMessage("Kullanıcı adı en az 3 karakter olmalıdır.")
            .MaximumLength(20).WithMessage("Kullanıcı adı en fazla 20 karakter olmalıdır.");
    }
}

public sealed class UserUpdateCommandHandler(
    IUserRepository userRepository,
    IClaimContext claimContext,
    IUnitOfWork unitOfWork) : IRequestHandler<UserUpdateCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UserUpdateCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (user is null)
        {
            return Result<string>.Failure("Kullanıcı bulunamadı.");
        }

        if (user.Email.Value != request.Email)
        {
            var emailExists = await userRepository.AnyAsync(x => x.Email.Value == request.Email, cancellationToken);
            if (emailExists)
            {
                return Result<string>.Failure("Bu e-posta adresi zaten kullanımda.");
            }
        }

        if (user.UserName.Value != request.UserName)
        {
            var userNameExists = await userRepository.AnyAsync(x => x.UserName.Value == request.UserName, cancellationToken);
            if (userNameExists)
            {
                return Result<string>.Failure("Bu kullanıcı adı zaten kullanımda.");
            }
        }

        var branchId = claimContext.GetBranchId();
        if (request.BranchId is not null)
        {
            branchId = request.BranchId.Value;
        }

        FirstName firstName = new(request.FirstName);
        LastName lastName = new(request.LastName);
        Email email = new(request.Email);
        UserName userName = new(request.UserName);
        IdentityId branchIdRecord = new(branchId);
        IdentityId roleId = new(request.RoleId);

        user.SetFirstName(firstName);
        user.SetLastName(lastName);
        user.SetFullName();
        user.SetEmail(email);
        user.SetUserName(userName);
        user.SetFullName();
        user.SetBranchId(branchIdRecord);
        user.SetRoleId(roleId);
        user.SetStatus(request.IsActive);

        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return "Kullanıcı başarıyla güncellendi";
    }
}
