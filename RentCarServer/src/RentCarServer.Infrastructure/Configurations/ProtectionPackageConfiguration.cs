using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentCarServer.Domain.ProtectionPackages;

namespace RentCarServer.Infrastructure.Configurations;
internal sealed class ProtectionPackageConfiguration : IEntityTypeConfiguration<ProtectionPackage>
{
    public void Configure(EntityTypeBuilder<ProtectionPackage> builder)
    {
        builder.ToTable("ProtectionPackages");
        builder.HasKey(p => p.Id);

        builder.OwnsOne(p => p.Name);
        builder.OwnsOne(p => p.Price);
        builder.OwnsOne(p => p.IsRecommended);
        builder.OwnsOne(p => p.OrderNumber);

        builder.OwnsMany(p => p.Coverages);
    }
}