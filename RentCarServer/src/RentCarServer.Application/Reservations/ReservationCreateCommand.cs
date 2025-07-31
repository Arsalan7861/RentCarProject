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

public sealed record CreditCardInformation(
    string CardNumber,
    string Owner,
    string ExpirationDate,
    string Cvv
);

[Permission("reservation:create")]
public sealed record ReservationCreateCommand(
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
    CreditCardInformation CreditCardInformation,
    decimal Total,
    int TotalDay
    ) : IRequest<Result<string>>;

public sealed class ReservationCreateCommandValidator : AbstractValidator<ReservationCreateCommand>
{
    public ReservationCreateCommandValidator()
    {
        RuleFor(x => x.CreditCardInformation.CardNumber)
            .NotEmpty().WithMessage("Kart numarası boş olamaz.");
        RuleFor(x => x.CreditCardInformation.Owner)
            .NotEmpty().WithMessage("Kart sahibi adı boş olamaz.");
        RuleFor(x => x.CreditCardInformation.ExpirationDate)
            .NotEmpty().WithMessage("Kart son kullanma tarihi boş olamaz.");
        RuleFor(x => x.CreditCardInformation.Cvv)
            .NotEmpty().WithMessage("Kart CVV kodu boş olamaz.");


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

public sealed class ReservationCreateCommandHandler(
    IVehicleRepository vehicleRepository,
    ICustomerRepository customerRepository,
    IBranchRepository branchRepository,
    IReservationRepository reservationRepository,
    IClaimContext claimContext,
    IUnitOfWork unitOfWork) : IRequestHandler<ReservationCreateCommand, Result<string>>
{
    public async Task<Result<string>> Handle(ReservationCreateCommand request, CancellationToken cancellationToken)
    {
        var locationId = request.PickUpLocationId ?? claimContext.GetBranchId();

        #region Şube, Müşteri ve Araç Kontrolü
        var branchExists = await branchRepository.AnyAsync(
            x => x.Id == locationId,
            cancellationToken);
        if (!branchExists)
            return Result<string>.Failure("Belirtilen teslimat noktası bulunamadı.");

        var customerExists = await customerRepository.AnyAsync(c => c.Id == request.CustomerId, cancellationToken);
        if (!customerExists)
            return Result<string>.Failure("Belirtilen müşteri bulunamadı.");

        var vehicleExists = await vehicleRepository.AnyAsync(v => v.Id == request.VehicleId, cancellationToken);
        if (!vehicleExists)
            return Result<string>.Failure("Belirtilen araç bulunamadı.");
        #endregion

        #region Araç Müsaitlik Kontrolü

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
        var last4Digits = request.CreditCardInformation.CardNumber[^4..];
        PaymenyInformation paymentInformation = new(last4Digits, request.CreditCardInformation.Owner);
        Status status = Status.Pending;
        Total total = new(request.Total);
        TotalDay totalDay = new(request.TotalDay);
        ReservationHistory history = new(
            "Rezervasyon Oluşturuldu",
            "Online olarak rezervasyon oluşturuldu.",
            DateTimeOffset.Now
            );

        Reservation reservation = Reservation.Create(
            customerId,
            pickUpLocationId,
            pickUpDate,
            pickUpTime,
            deliveryDate,
            deliveryTime,
            vehicleId,
            vehicleDailyPrice,
            protectionPackageId,
            protectionPackagePrice,
            reservationExtras,
            note,
            paymentInformation,
            status,
            total,
            totalDay,
            history
            );
        #endregion

        #region Ödeme İşlemi
        // Ödeme işlemi burada yapılabilir. Örnek olarak, kredi kartı bilgilerini doğrulama veya ödeme API'si ile entegrasyon.
        ReservationHistory history2 = new(
            "Ödeme Alındı",
            "Rezervasyonun ödemesi başarıyla alındı.",
            DateTimeOffset.Now
            );
        reservation.SetHistory(history2);
        #endregion
        await reservationRepository.AddAsync(reservation, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return "Rezervasyon başarıyla oluşturuldu";
    }
}
