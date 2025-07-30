using RentCarServer.Domain.Abstractions;
using RentCarServer.Domain.Customers.ValueObjects;
using RentCarServer.Domain.Shared;

namespace RentCarServer.Domain.Customers;
public sealed class Customer : Entity
{
    private Customer()
    {
    }

    public Customer(
        FirstName firstName,
        LastName lastName,
        IdentityNumber identityNumber,
        BirthDate birthDate,
        PhoneNumber phoneNumber,
        Email email,
        DrivingLicenseIssuanceDate drivingLicenseIssuanceDate,
        FullAddress fullAddress,
        Password password,
        bool isACtive)
    {
        SetFirstName(firstName);
        SetLastName(lastName);
        SetFullName();
        SetIdentityNumber(identityNumber);
        SetBirthDate(birthDate);
        SetPhoneNumber(phoneNumber);
        SetEmail(email);
        SetDrivingLicenseIssuanceDate(drivingLicenseIssuanceDate);
        SetFullAddress(fullAddress);
        SetPassword(password);
        SetStatus(isACtive);
    }

    public FirstName FirstName { get; private set; } = default!;
    public LastName LastName { get; private set; } = default!;
    public FullName FullName { get; private set; }
    public IdentityNumber IdentityNumber { get; private set; } = default!;
    public BirthDate BirthDate { get; private set; } = default!;
    public PhoneNumber PhoneNumber { get; private set; } = default!;
    public Email Email { get; private set; } = default!;
    public DrivingLicenseIssuanceDate DrivingLicenseIssuanceDate { get; private set; } = default!;
    public FullAddress FullAddress { get; private set; } = default!;
    public Password Password { get; private set; }

    #region Behaviors

    public void SetFirstName(FirstName firstName)
    {
        FirstName = firstName;
    }
    public void SetLastName(LastName lastName)
    {
        LastName = lastName;
    }
    public void SetFullName()
    {
        FullName = new(string.Join(" ", FirstName.Value, LastName.Value));
    }
    public void SetIdentityNumber(IdentityNumber tcNumber)
    {
        IdentityNumber = tcNumber;
    }
    public void SetBirthDate(BirthDate birthDate)
    {
        BirthDate = birthDate;
    }
    public void SetPhoneNumber(PhoneNumber phoneNumber)
    {
        PhoneNumber = phoneNumber;
    }
    public void SetEmail(Email email)
    {
        Email = email;
    }
    public void SetDrivingLicenseIssuanceDate(DrivingLicenseIssuanceDate licenseDate)
    {
        DrivingLicenseIssuanceDate = licenseDate;
    }
    public void SetFullAddress(FullAddress fullAddress)
    {
        FullAddress = fullAddress;
    }
    public void SetPassword(Password password)
    {
        Password = password;
    }
    public bool VerifyPasswordHash(string password)
    {
        using var hmac = new System.Security.Cryptography.HMACSHA512(Password.PasswordSalt);
        var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return computedHash.SequenceEqual(Password.PasswordHash);
    }

    #endregion
}
