using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentCarServer.Domain.Extras;

namespace RentCarServer.Infrastructure.Configurations;
internal sealed class ExtraConfiguration : IEntityTypeConfiguration<Extra>
{
    public void Configure(EntityTypeBuilder<Extra> builder)
    {
        builder.ToTable("Extras");
        builder.HasKey(e => e.Id);
        builder.OwnsOne(e => e.Name);
        builder.OwnsOne(e => e.Price);
        builder.OwnsOne(e => e.Description);
    }
}