using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentCarServer.Domain.Reservations;

namespace RentCarServer.Infrastructure.Configurations;
internal sealed class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("Reservations");
        builder.HasKey(r => r.Id);

        builder.OwnsOne(r => r.ReservationNumber);
        builder.OwnsOne(r => r.PickUpDate);
        builder.OwnsOne(r => r.PickUpTime);
        builder.OwnsOne(r => r.DeliveryDate);
        builder.OwnsOne(r => r.DeliveryTime);
        builder.OwnsOne(r => r.TotalDay);
        builder.OwnsOne(r => r.VehicleDailyPrice);
        builder.OwnsOne(r => r.ProtectionPackagePrice);
        builder.OwnsMany(r => r.ReservationExtras);
        builder.OwnsOne(r => r.Note);
        builder.OwnsOne(r => r.PaymentInformation);
        builder.OwnsOne(r => r.Status);
        builder.OwnsOne(r => r.Total);
        builder.OwnsOne(r => r.PickUpDateTime);
        builder.OwnsOne(r => r.DeliveryDateTime);
        builder.OwnsMany(r => r.Histories);
    }
}
