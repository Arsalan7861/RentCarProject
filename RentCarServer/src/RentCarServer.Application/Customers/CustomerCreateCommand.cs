using FluentValidation;
using GenericRepository;
using RentCarServer.Application.Behaviors;
using RentCarServer.Domain.Customers;
using RentCarServer.Domain.Customers.ValueObjects;
using RentCarServer.Domain.Shared;
using TS.MediatR;
using TS.Result;

namespace RentCarServer.Application.Customers;

[Permission("customer:create")]
public sealed record CustomerCreateCommand(
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

public sealed class CustomerCreateCommandValidator : AbstractValidator<CustomerCreateCommand>
{
    public CustomerCreateCommandValidator()
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

public sealed class CustomerCreateCommandHandler(
    ICustomerRepository customerRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CustomerCreateCommand, Result<string>>
{

    public async Task<Result<string>> Handle(CustomerCreateCommand request, CancellationToken cancellationToken)
    {
        var existingCustomer = await customerRepository.AnyAsync(
            x => x.IdentityNumber.Value == request.IdentityNumber, cancellationToken);
        if (existingCustomer)
            return Result<string>.Failure("Bu kimlik numarası ile kayıtlı müşteri var.");

        FirstName firstName = new(request.FirstName);
        LastName lastName = new(request.LastName);
        IdentityNumber identityNumber = new(request.IdentityNumber);
        BirthDate birthDate = new(request.BirthDate);
        PhoneNumber phoneNumber = new(request.PhoneNumber);
        Email email = new(request.Email);
        DrivingLicenseIssuanceDate drivingLicenseIssuanceDate = new(request.DrivingLicenseIssuanceDate);
        FullAddress fullAddress = new(request.FullAddress);
        Password password = new("123");

        var customer = new Customer(
            firstName,
            lastName,
            identityNumber,
            birthDate,
            phoneNumber,
            email,
            drivingLicenseIssuanceDate,
            fullAddress,
            password,
            request.IsActive);

        await customerRepository.AddAsync(customer, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return "Müşteri başarıyla oluşturuldu.";
    }
}
