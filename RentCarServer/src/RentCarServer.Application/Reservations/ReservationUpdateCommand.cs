using FluentValidation;
using GenericRepository;
using Microsoft.EntityFrameworkCore;
using RentCarServer.Application.Behaviors;
using RentCarServer.Application.Services;
using RentCarServer.Domain.Abstractions;
using RentCarServer.Domain.Branches;
using RentCarServer.Domain.Customers;
using RentCarServer.Domain.Reservations;
using RentCarServer.Domain.Reservations.ValueObjects;
using RentCarServer.Domain.Shared;
using RentCarServer.Domain.Vehicles;
using TS.MediatR;
using TS.Result;
namespace RentCarServer.Application.Reservations;

[Permission("reservation:edit")]
public sealed record ReservationUpdateCommand(
    Guid Id,
    Guid CustomerId,
    Guid? PickUpLocationId,
    DateOnly PickUpDate,
    TimeOnly PickUpTime,
    DateOnly DeliveryDate,
    TimeOnly DeliveryTime,
    Guid VehicleId,
    decimal VehicleDailyPrice,
    Guid ProtectionPackageId,
    decimal ProtectionPackagePrice,
    List<ReservationExtra> ReservationExtras,
    string Note,
    decimal Total,
    int TotalDay
    ) : IRequest<Result<string>>;

public sealed class ReservationUpdateCommandValidator : AbstractValidator<ReservationUpdateCommand>
{
    public ReservationUpdateCommandValidator()
    {
        RuleFor(x => x.PickUpDate)
            .NotEmpty().WithMessage("Alış tarihi boş olamaz.")
            .Must(date => date >= DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Alış tarihi geçmiş bir tarih olamaz.");

        RuleFor(x => x.DeliveryDate)
            .NotEmpty().WithMessage("Teslim tarihi boş olamaz.")
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Teslim tarihi bugünden önce olamaz.")
            .Must((cmd, deliveryDate) => deliveryDate >= cmd.PickUpDate)
            .WithMessage("Teslim tarihi alış tarihinden önce olamaz.");
    }
}

public sealed class ReservationUpdateCommandHandler(
    IVehicleRepository vehicleRepository,
    ICustomerRepository customerRepository,
    IBranchRepository branchRepository,
    IReservationRepository reservationRepository,
    IClaimContext claimContext,
    IUnitOfWork unitOfWork) : IRequestHandler<ReservationUpdateCommand, Result<string>>
{
    public async Task<Result<string>> Handle(ReservationUpdateCommand request, CancellationToken cancellationToken)
    {
        Reservation? reservation = await reservationRepository.FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
        if (reservation == null)
            return Result<string>.Failure("Rezervasyon bulunamadı.");

        if (reservation.Status == Status.Completed || reservation.Status == Status.Canceled)
            return Result<string>.Failure("Tamamlanmış veya iptal edilmiş rezervasyon güncellenemez.");

        var locationId = request.PickUpLocationId ?? claimContext.GetBranchId();

        #region Şube, Müşteri ve Araç Kontrolü
        if (reservation.PickUpLocationId.Value != locationId)
        {
            // Eğer teslimat noktası değiştiyse, yeni teslimat noktasının varlığını kontrol et
            var branchExists = await branchRepository.AnyAsync(
                x => x.Id == locationId,
                cancellationToken);
            if (!branchExists)
                return Result<string>.Failure("Belirtilen teslimat noktası bulunamadı.");
        }

        if (reservation.CustomerId != request.CustomerId)
        {
            // Eğer müşteri değiştiyse, yeni müşterinin varlığını kontrol et
            var customerExists = await customerRepository.AnyAsync(c => c.Id == request.CustomerId, cancellationToken);
            if (!customerExists)
                return Result<string>.Failure("Belirtilen müşteri bulunamadı.");
        }

        if (reservation.VehicleId != request.VehicleId)
        {
            // Eğer araç değiştiyse, yeni aracın varlığını kontrol et
            var vehicleExists = await vehicleRepository.AnyAsync(v => v.Id == request.VehicleId, cancellationToken);
            if (!vehicleExists)
                return Result<string>.Failure("Belirtilen araç bulunamadı.");
        }
        #endregion

        #region Araç Müsaitlik Kontrolü
        if (reservation.PickUpDate.Value != request.PickUpDate ||
           reservation.PickUpTime.Value != request.PickUpTime ||
           reservation.DeliveryDate.Value != request.DeliveryDate ||
           reservation.DeliveryTime.Value != request.DeliveryTime)
        {
            var requestedPickUpDate = request.PickUpDate.ToDateTime(request.PickUpTime);
            var requestedDeliveryDate = request.DeliveryDate.ToDateTime(request.DeliveryTime);

            var possibleOverlaps = await reservationRepository
                .Where(r => r.VehicleId == request.VehicleId
                && (r.Status.Value == Status.Pending.Value || r.Status.Value == Status.Delivered.Value))
                .Select(s => new
                {
                    Id = s.Id,
                    VehicleId = s.VehicleId,
                    DeliveryDate = s.DeliveryDate.Value,
                    DeliveryTime = s.DeliveryTime.Value,
                    PickUpDate = s.PickUpDate.Value,
                    PickUpTime = s.PickUpTime.Value,
                })
                .ToListAsync(cancellationToken);

            var overlaps = possibleOverlaps.Any(r =>
                requestedPickUpDate < r.DeliveryDate.ToDateTime(r.DeliveryTime).AddHours(1) &&
                requestedDeliveryDate > r.PickUpDate.ToDateTime(r.PickUpTime)
            );

            if (overlaps)
                return Result<string>.Failure("Seçilen araç, belirtilen tarih ve saat aralığında müsait değil.");
        }

        #endregion

        #region Reservation Objesini Oluşturma

        IdentityId customerId = new(request.CustomerId);
        IdentityId pickUpLocationId = new(locationId);
        PickUpDate pickUpDate = new(request.PickUpDate);
        PickUpTime pickUpTime = new(request.PickUpTime);
        DeliveryDate deliveryDate = new(request.DeliveryDate);
        DeliveryTime deliveryTime = new(request.DeliveryTime);
        IdentityId vehicleId = new(request.VehicleId);
        Price vehicleDailyPrice = new(request.VehicleDailyPrice);
        IdentityId protectionPackageId = new(request.ProtectionPackageId);
        Price protectionPackagePrice = new(request.ProtectionPackagePrice);
        IEnumerable<ReservationExtra> reservationExtras = request.ReservationExtras
            .Select(extra => new ReservationExtra(
                extra.ExtraId,
                extra.Price));
        Note note = new(request.Note);
        Total total = new(request.Total);
        TotalDay totalDay = new(request.TotalDay);

        reservation.SetCustomerId(customerId);
        reservation.SetPickUpLocationId(pickUpLocationId);
        reservation.SetPickUpDate(pickUpDate);
        reservation.SetPickUpTime(pickUpTime);
        reservation.SetDeliveryDate(deliveryDate);
        reservation.SetDeliveryTime(deliveryTime);
        reservation.SetTotalDay(totalDay);
        reservation.SetVehicleId(vehicleId);
        reservation.SetVehicleDailyPrice(vehicleDailyPrice);
        reservation.SetProtectionPackageId(protectionPackageId);
        reservation.SetProtectionPackagePrice(protectionPackagePrice);
        reservation.SetReservationExtras(reservationExtras);
        reservation.SetNote(note);
        reservation.SetTotal(total);
        reservation.SetPickUpDateTime();
        reservation.SetDeliveryDateTime();

        #endregion

        reservationRepository.Update(reservation);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return "Rezervasyon başarıyla güncellendi";
    }
}
