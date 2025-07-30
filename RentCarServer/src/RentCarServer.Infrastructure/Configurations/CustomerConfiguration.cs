using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentCarServer.Domain.Customers;

namespace RentCarServer.Infrastructure.Configurations;
internal sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");
        builder.HasKey(c => c.Id);

        builder.OwnsOne(c => c.FirstName);
        builder.OwnsOne(c => c.LastName);
        builder.OwnsOne(c => c.FullName);
        builder.OwnsOne(c => c.IdentityNumber);
        builder.OwnsOne(c => c.BirthDate);
        builder.OwnsOne(c => c.PhoneNumber);
        builder.OwnsOne(c => c.Email);
        builder.OwnsOne(c => c.DrivingLicenseIssuanceDate);
        builder.OwnsOne(c => c.FullAddress);
        builder.OwnsOne(c => c.Password);
    }
}
