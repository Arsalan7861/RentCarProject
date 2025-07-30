using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentCarServer.Domain.Vehicles;

namespace RentCarServer.Infrastructure.Configurations;
internal sealed class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("Vehicles");
        builder.HasKey(v => v.Id);

        builder.OwnsOne(v => v.Brand);
        builder.OwnsOne(v => v.Model);
        builder.OwnsOne(v => v.ModelYear);
        builder.OwnsOne(v => v.Color);
        builder.OwnsOne(v => v.Plate);
        builder.OwnsOne(v => v.CategoryId);
        builder.OwnsOne(v => v.BranchId);
        builder.OwnsOne(v => v.VinNumber);
        builder.OwnsOne(v => v.EngineNumber);
        builder.OwnsOne(v => v.Description);
        builder.OwnsOne(v => v.ImageUrl);
        builder.OwnsOne(v => v.FuelType);
        builder.OwnsOne(v => v.Transmission);

        builder.OwnsOne(v => v.EngineVolume, x => x.Property(e => e.Value).HasColumnType("decimal(18,2)"));
        builder.OwnsOne(v => v.FuelConsumption, x => x.Property(e => e.Value).HasColumnType("decimal(18,2)"));
        builder.OwnsOne(v => v.DailyPrice);
        builder.OwnsOne(v => v.WeeklyDiscountRate, x => x.Property(e => e.Value).HasColumnType("decimal(18,2)"));
        builder.OwnsOne(v => v.MonthlyDiscountRate, x => x.Property(e => e.Value).HasColumnType("decimal(18,2)"));

        builder.OwnsOne(v => v.EnginePower);
        builder.OwnsOne(v => v.TractionType);
        builder.OwnsOne(v => v.SeatCount);
        builder.OwnsOne(v => v.Kilometer);
        builder.OwnsOne(v => v.InsuranceType);
        builder.OwnsOne(v => v.LastMaintenanceDate);
        builder.OwnsOne(v => v.LastMaintenanceKm);
        builder.OwnsOne(v => v.NextMaintenanceKm);
        builder.OwnsOne(v => v.InspectionDate);
        builder.OwnsOne(v => v.InsuranceEndDate);
        builder.OwnsOne(v => v.CascoEndDate);
        builder.OwnsOne(v => v.TireStatus);
        builder.OwnsOne(v => v.GeneralStatus);
        builder.OwnsMany(v => v.Features);
    }
}