using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentCarServer.Domain.LoginTokens;

namespace RentCarServer.Infrastructure.Configurations;
internal sealed class LoginTokenConfiguration : IEntityTypeConfiguration<LoginToken>
{
    public void Configure(EntityTypeBuilder<LoginToken> builder)
    {
        builder.ToTable("LoginTokens");
        builder.HasKey(i => i.Id);
        builder.OwnsOne(i => i.IsActive);
        builder.OwnsOne(i => i.Token);
        builder.OwnsOne(i => i.ExpiresDate);
    }
}
