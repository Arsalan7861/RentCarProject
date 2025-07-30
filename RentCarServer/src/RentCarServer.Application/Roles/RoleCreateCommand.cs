using FluentValidation;
using GenericRepository;
using RentCarServer.Application.Behaviors;
using RentCarServer.Domain.Roles;
using RentCarServer.Domain.Shared;
using TS.MediatR;
using TS.Result;

namespace RentCarServer.Application.Roles;
[Permission("role:create")]
public sealed record RoleCreateCommand(
    string Name,
    bool IsActive) : IRequest<Result<string>>;

internal sealed class RoleCreateCommandValidator : AbstractValidator<RoleCreateCommand>
{
    public RoleCreateCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Geçerli bir rol adı girin.");
    }
}

internal sealed class RoleCreateCommandHandler(
    IRoleRepository roleRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<RoleCreateCommand, Result<string>>
{
    public async Task<Result<string>> Handle(RoleCreateCommand request, CancellationToken cancellationToken)
    {
        var nameExists = await roleRepository.AnyAsync(x => x.Name.Value == request.Name, cancellationToken);
        if (nameExists)
        {
            return Result<string>.Failure("Bu isimde bir rol zaten mevcut.");
        }

        Name name = new(request.Name);
        var role = new Role(name, request.IsActive);
        await roleRepository.AddAsync(role);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return "Rol başarıyla oluşturuldu.";
    }
}