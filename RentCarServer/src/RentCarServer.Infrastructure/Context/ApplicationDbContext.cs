using GenericRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RentCarServer.Domain.Abstractions;
using System.Security.Claims;

namespace RentCarServer.Infrastructure.Context;
internal sealed class ApplicationDbContext : DbContext, IUnitOfWork
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly); // Entity sınıflarının konfigürasyonlarını uygula, bu sayede entity sınıflarının konfigürasyonlarını ayrı bir dosyada tutabiliriz.
        modelBuilder.ApplyGlobalFilters(); // Filtreyi uygula, IsDeleted alanı false olanları getirir
        base.OnModelCreating(modelBuilder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<IdentityId>().HaveConversion<IdentityIdValueConverter>(); // IdentityId veri tipli özellikler için dönüşüm uygula, entityFramework IdentityId türünü bilmiyor bu yüzden ona söylenmesi lazım.
        configurationBuilder.Properties<decimal>().HaveColumnType("money"); // decimal veri tipli özellikler için dönüşüm uygula, veritabanında money olarak saklanacak
        configurationBuilder.Properties<string>()
            .HaveColumnType("nvarchar(MAX)"); // string veri tipli özellikler için dönüşüm uygula, veritabanında varchar(MAX) olarak saklanacak
        configurationBuilder.Properties<TimeOnly>()
            .HaveColumnType("time(7)");
        base.ConfigureConventions(configurationBuilder);
    }

    // SaveChangesAsync metodunu override ediyoruz, böylece veritabanına veri kaydetmeden önce bazı işlemleri yapabiliriz. veriyi kaydetmeden önce Audit işlemlerini yapacağız.
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<Entity>();

        HttpContextAccessor httpContextAccessor = new();
        string? userIdString =
        httpContextAccessor
            .HttpContext?
            .User
            .Claims
            .FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier)?
            .Value;

        if (string.IsNullOrEmpty(userIdString))
        {
            return base.SaveChangesAsync(cancellationToken);
        }

        Guid userId = Guid.Parse(userIdString);
        IdentityId identityId = new(userId);

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Property(p => p.CreatedAt)
                    .CurrentValue = DateTimeOffset.Now;
                entry.Property(p => p.CreatedBy)
                    .CurrentValue = identityId;
            }

            if (entry.State == EntityState.Modified)
            {
                if (entry.Property(p => p.IsDeleted).CurrentValue == true)
                {
                    entry.Property(p => p.DeletedAt)
                    .CurrentValue = DateTimeOffset.Now;
                    entry.Property(p => p.DeletedBy)
                    .CurrentValue = identityId;
                }
                else
                {
                    entry.Property(p => p.UpdatedAt)
                        .CurrentValue = DateTimeOffset.Now;
                    entry.Property(p => p.UpdatedBy)
                    .CurrentValue = identityId;
                }
            }

            if (entry.State == EntityState.Deleted)
            {
                throw new ArgumentException("Db'den direkt silme işlemi yapamazsınız");
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}

internal sealed class IdentityIdValueConverter : ValueConverter<IdentityId, Guid>
{ // IdentityId türünü Guid türüne dönüştürmek için ValueConverter sınıfını kullanıyoruz
    public IdentityIdValueConverter() : base(m => m.Value, m => new IdentityId(m)) { }
}
