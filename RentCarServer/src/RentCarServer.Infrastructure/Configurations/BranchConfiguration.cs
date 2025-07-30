using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentCarServer.Domain.Branches;

namespace RentCarServer.Infrastructure.Configurations;
internal sealed class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.ToTable("Branches");
        builder.HasKey(b => b.Id);
        builder.OwnsOne(b => b.Name);
        builder.OwnsOne(b => b.Address);
        builder.OwnsOne(b => b.Contact);
    }
}
