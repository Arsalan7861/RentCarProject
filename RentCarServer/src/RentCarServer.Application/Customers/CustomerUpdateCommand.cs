using FluentValidation;
using GenericRepository;
using RentCarServer.Application.Behaviors;
using RentCarServer.Domain.Customers;
using RentCarServer.Domain.Customers.ValueObjects;
using RentCarServer.Domain.Shared;
using TS.MediatR;
using TS.Result;

namespace RentCarServer.Application.Customers;
[Permission("customer:edit")]
public sealed record CustomerUpdateCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string IdentityNumber,
    DateOnly BirthDate,
    string PhoneNumber,
    string Email,
    DateOnly DrivingLicenseIssuanceDate,
    string FullAddress,
    bool IsActive
) : IRequest<Result<string>>;

public sealed class CustomerUpdateCommandValidator : AbstractValidator<CustomerUpdateCommand>
{
    public CustomerUpdateCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Ad boş olamaz.");
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Soyad boş olamaz.");
        RuleFor(x => x.IdentityNumber)
            .NotEmpty().WithMessage("Kimlik numarası boş olamaz.")
            .Length(11).WithMessage("Kimlik numarası 11 haneli olmalıdır.");
        RuleFor(x => x.BirthDate)
            .NotEmpty().WithMessage("Doğum tarihi boş olamaz.")
            .Must(date => date <= DateOnly.FromDateTime(DateTime.Now)).WithMessage("Doğum tarihi gelecekte olamaz.");
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Telefon numarası boş olamaz.");
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-posta adresi boş olamaz.")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi girin.");
        RuleFor(x => x.DrivingLicenseIssuanceDate)
            .NotEmpty().WithMessage("Ehliyet verme tarihi boş olamaz.")
            .Must(date => date <= DateOnly.FromDateTime(DateTime.Now)).WithMessage("Ehliyet verme tarihi gelecekte olamaz.");
        RuleFor(x => x.FullAddress)
            .NotEmpty().WithMessage("Adres boş olamaz.");
    }
}

public sealed class CustomerUpdateCommandHandler(
    ICustomerRepository customerRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CustomerUpdateCommand, Result<string>>
{
    public async Task<Result<string>> Handle(CustomerUpdateCommand request, CancellationToken cancellationToken)
    {
        var customer = await customerRepository.FirstOrDefaultAsync(x => x.Id == request.Id);
        if (customer is null)
            return Result<string>.Failure("Müşteri bulunamadı.");

        FirstName firstName = new(request.FirstName);
        LastName lastName = new(request.LastName);
        IdentityNumber identityNumber = new(request.IdentityNumber);
        BirthDate birthDate = new(request.BirthDate);
        PhoneNumber phoneNumber = new(request.PhoneNumber);
        Email email = new(request.Email);
        DrivingLicenseIssuanceDate drivingLicenseIssuanceDate = new(request.DrivingLicenseIssuanceDate);
        FullAddress fullAddress = new(request.FullAddress);

        customer.SetFirstName(firstName);
        customer.SetLastName(lastName);
        customer.SetFullName();
        customer.SetIdentityNumber(identityNumber);
        customer.SetBirthDate(birthDate);
        customer.SetPhoneNumber(phoneNumber);
        customer.SetEmail(email);
        customer.SetDrivingLicenseIssuanceDate(drivingLicenseIssuanceDate);
        customer.SetFullAddress(fullAddress);
        customer.SetStatus(request.IsActive);

        customerRepository.Update(customer);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return "Müşteri bilgileri başarıyla güncellendi";
    }
}