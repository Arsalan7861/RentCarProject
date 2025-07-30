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
[Permission("user:create")]
public sealed record UserCreateCommand(
    string FirstName,
    string LastName,
    string Email,
    string UserName,
    Guid? BranchId,
    Guid RoleId,
    bool IsActive) : IRequest<Result<string>>;

public sealed class UserCreateCommandValidator : AbstractValidator<UserCreateCommand>
{
    public UserCreateCommandValidator()
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

public sealed class UserCreateCommandHandler(
    IUserRepository userRepository,
    IClaimContext claimContext,
    IUnitOfWork unitOfWork) : IRequestHandler<UserCreateCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UserCreateCommand request, CancellationToken cancellationToken)
    {
        var emailExists = await userRepository.AnyAsync(x => x.Email.Value == request.Email, cancellationToken);
        if (emailExists)
        {
            return Result<string>.Failure("Bu e-posta adresi zaten kullanımda.");
        }

        var userNameExists = await userRepository.AnyAsync(x => x.UserName.Value == request.UserName, cancellationToken);
        if (userNameExists)
        {
            return Result<string>.Failure("Bu kullanıcı adı zaten kullanımda.");
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
        Password password = new("123");
        IdentityId branchIdRecord = new(branchId);
        IdentityId roleId = new(request.RoleId);

        User user = new(firstName, lastName, email, userName, password, branchIdRecord, roleId, request.IsActive);

        await userRepository.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return "Kullanıcı başarıyla oluşturuldu";
    }
}
