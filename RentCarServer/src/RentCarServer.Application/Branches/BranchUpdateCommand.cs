using FluentValidation;
using GenericRepository;
using RentCarServer.Application.Behaviors;
using RentCarServer.Domain.Branches;
using RentCarServer.Domain.Shared;
using TS.MediatR;
using TS.Result;

namespace RentCarServer.Application.Branches;
[Permission("branch:edit")]
public sealed record BranchUpdateCommand(
    Guid Id,
    string Name,
    Address Address,
    Contact Contact,
    bool IsActive) : IRequest<Result<string>>;


public sealed class BranchUpdateCommandValidator : AbstractValidator<BranchCreateCommand>
{
    public BranchUpdateCommandValidator()
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

public sealed class BranchUpdateCommandHandler(
    IBranchRepository branchRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<BranchUpdateCommand, Result<string>>
{
    public async Task<Result<string>> Handle(BranchUpdateCommand request, CancellationToken cancellationToken)
    {
        var branch = await branchRepository.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (branch is null)
        {
            return Result<string>.Failure("Şube bulunamadı.");
        }

        Address address = request.Address;
        Contact contact = request.Contact;

        branch.SetName(new Name(request.Name));
        branch.SetAddress(address);
        branch.SetContact(contact);
        branch.SetStatus(request.IsActive);
        branchRepository.Update(branch);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return "Şube bilgisi başarıyla güncellendi.";
    }
}
