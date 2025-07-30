using FluentValidation;
using GenericRepository;
using RentCarServer.Application.Behaviors;
using RentCarServer.Domain.Branches;
using RentCarServer.Domain.Shared;
using TS.MediatR;
using TS.Result;

namespace RentCarServer.Application.Branches;
[Permission("branch:create")]
public sealed record BranchCreateCommand(
    string Name,
    Address Address,
    Contact Contact,
    bool IsActive) : IRequest<Result<string>>;

public sealed class BranchCreateCommandValidator : AbstractValidator<BranchCreateCommand>
{
    public BranchCreateCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Geçerli bir şube adı girin.");
        RuleFor(x => x.Address.City)
            .NotEmpty()
            .WithMessage("Geçerli bir şehir seçin.");
        RuleFor(x => x.Address.District)
            .NotEmpty()
            .WithMessage("Geçerli bir ilçe seçin.");
        RuleFor(x => x.Address.FullAddress)
            .NotEmpty()
            .WithMessage("Geçerli bir tam adres girin.");
        RuleFor(x => x.Contact.PhoneNumber1)
            .NotEmpty()
            .WithMessage("Geçerli bir telefon numarası girin.");
    }
}

public sealed class BranchCreateCommandHandler(
    IBranchRepository branchRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<BranchCreateCommand, Result<string>>
{
    public async Task<Result<string>> Handle(BranchCreateCommand request, CancellationToken cancellationToken)
    {
        var nameIsExists = await branchRepository.AnyAsync(x => x.Name.Value == request.Name, cancellationToken);
        if (nameIsExists)
        {
            return Result<string>.Failure("Bu isimde bir şube zaten var.");
        }

        Name name = new(request.Name);
        Address address = request.Address;
        Contact contact = request.Contact;
        Branch branch = new(name, address, contact, request.IsActive);
        await branchRepository.AddAsync(branch, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return "Şube başarıyla oluşturuldu.";
    }
}
