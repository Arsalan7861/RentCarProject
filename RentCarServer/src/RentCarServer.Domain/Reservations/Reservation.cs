using RentCarServer.Domain.Abstractions;
using RentCarServer.Domain.Reservations.Forms;
using RentCarServer.Domain.Reservations.ValueObjects;
using RentCarServer.Domain.Shared;

namespace RentCarServer.Domain.Reservations;
public sealed class Reservation : Entity, IAggregate
{
    private readonly List<ReservationExtra> _reservationExtras = new();
    private readonly List<ReservationHistory> _histories = new();
    private Reservation()
    {
        // Required for EF Core
    }
    private Reservation(
        IdentityId customerId,
        IdentityId pickUpLocationId,
        PickUpDate pickUpDate,
        PickUpTime pickUpTime,
        DeliveryDate deliveryDate,
        DeliveryTime deliveryTime,
        IdentityId vehicleId,
        Price vehicleDailyPrice,
        IdentityId protectionPackageId,
        Price protectionPackagePrice,
        IEnumerable<ReservationExtra> reservationExtras,
        Note note,
        PaymenyInformation paymentInformation,
        Status status,
        Total total,
        TotalDay totalDay,
        ReservationHistory history,
        Form pickUpForm,
        Form deliveryForm
        )
    {
        SetCustomerId(customerId);
        SetPickUpLocationId(pickUpLocationId);
        SetPickUpDate(pickUpDate);
        SetPickUpTime(pickUpTime);
        SetDeliveryDate(deliveryDate);
        SetDeliveryTime(deliveryTime);
        SetTotalDay(totalDay);
        SetVehicleId(vehicleId);
        SetVehicleDailyPrice(vehicleDailyPrice);
        if (protectionPackageId is not null)
        {
            SetProtectionPackageId(protectionPackageId);
        }
        if (protectionPackagePrice is not null)
        {
            SetProtectionPackagePrice(protectionPackagePrice);
        }
        SetReservationExtras(reservationExtras);
        SetNote(note);
        SetPaymentInformation(paymentInformation);
        SetReservationStatus(status);
        SetTotal(total);
        SetPickUpDateTime();
        SetDeliveryDateTime();
        SetReservationNumber();
        SetHistory(history);
        SetPickUpForm(pickUpForm);
        SetDeliveryForm(deliveryForm);
    }
    public ReservationNumber ReservationNumber { get; private set; } = default!;

    public IdentityId CustomerId { get; private set; } = default!;
    public IdentityId PickUpLocationId { get; private set; } = default!;
    public PickUpDate PickUpDate { get; private set; } = default!;
    public PickUpTime PickUpTime { get; private set; } = default!;
    public PickUpDateTime PickUpDateTime { get; private set; } = default!;
    public DeliveryDate DeliveryDate { get; private set; } = default!;
    public DeliveryTime DeliveryTime { get; private set; } = default!;
    public DeliveryDateTime DeliveryDateTime { get; private set; } = default!;
    public TotalDay TotalDay { get; private set; } = default!;
    public IdentityId VehicleId { get; private set; } = default!;
    public Price VehicleDailyPrice { get; private set; } = default!;
    public IdentityId ProtectionPackageId { get; private set; } = default!;
    public Price ProtectionPackagePrice { get; private set; } = default!;
    public IReadOnlyCollection<ReservationExtra> ReservationExtras => _reservationExtras;
    public Note Note { get; private set; } = default!;
    public PaymenyInformation PaymentInformation { get; private set; } = default!;
    public Status Status { get; private set; } = default!;
    public Total Total { get; private set; } = default!;
    public IReadOnlyCollection<ReservationHistory> Histories => _histories;
    public Form PickUpForm { get; private set; } = default!;
    public Form DeliveryForm { get; private set; } = default!;

    public static Reservation Create(
        IdentityId customerId,
        IdentityId pickUpLocationId,
        PickUpDate pickUpDate,
        PickUpTime pickUpTime,
        DeliveryDate deliveryDate,
        DeliveryTime deliveryTime,
        IdentityId vehicleId,
        Price vehicleDailyPrice,
        IdentityId protectionPackageId,
        Price protectionPackagePrice,
        IEnumerable<ReservationExtra> reservationExtras,
        Note note,
        PaymenyInformation paymentInformation,
        Status status,
        Total total,
        TotalDay totalDay,
        ReservationHistory history,
        Form pickUpForm,
        Form deliveryForm
        )
    {
        return new Reservation(
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
            history,
            pickUpForm,
            deliveryForm
            );
    }
    #region Behaviors

    public void SetCustomerId(IdentityId customerId)
    {
        CustomerId = customerId;
    }
    public void SetPickUpLocationId(IdentityId pickUpLocationId)
    {
        PickUpLocationId = pickUpLocationId;
    }
    public void SetPickUpDate(PickUpDate pickUpDate)
    {
        PickUpDate = pickUpDate;
    }
    public void SetPickUpTime(PickUpTime pickUpTime)
    {
        PickUpTime = pickUpTime;
    }
    public void SetPickUpDateTime()
    {
        var date = new DateTime(PickUpDate.Value, PickUpTime.Value);
        PickUpDateTime = new(new DateTimeOffset(date));
    }
    public void SetDeliveryDate(DeliveryDate deliveryDate)
    {
        DeliveryDate = deliveryDate;
    }
    public void SetDeliveryTime(DeliveryTime deliveryTime)
    {
        DeliveryTime = deliveryTime;
    }
    public void SetDeliveryDateTime()
    {
        var date = new DateTime(DeliveryDate.Value, DeliveryTime.Value);
        DeliveryDateTime = new(new DateTimeOffset(date));
    }
    public void SetTotalDay(TotalDay totalDay)
    {
        TotalDay = totalDay;
    }
    public void SetVehicleId(IdentityId vehicleId)
    {
        VehicleId = vehicleId;
    }
    public void SetVehicleDailyPrice(Price vehicleDailyPrice)
    {
        VehicleDailyPrice = vehicleDailyPrice;
    }
    public void SetProtectionPackageId(IdentityId protectionPackageId)
    {
        ProtectionPackageId = protectionPackageId;
    }
    public void SetProtectionPackagePrice(Price protectionPackagePrice)
    {
        ProtectionPackagePrice = protectionPackagePrice;
    }
    public void SetReservationExtras(IEnumerable<ReservationExtra> reservationExtras)
    {
        _reservationExtras.Clear();
        _reservationExtras.AddRange(reservationExtras);
    }
    public void SetNote(Note note)
    {
        Note = note;
    }
    public void SetPaymentInformation(PaymenyInformation paymentInformation)
    {
        PaymentInformation = paymentInformation;
    }
    public void SetReservationStatus(Status status)
    {
        Status = status;
    }
    public void SetTotal(Total total)
    {
        Total = total;
    }
    private void SetReservationNumber()
    {
        var date = DateTime.Now;
        Random random = new();
        string reservationNumber = string.Concat(Enumerable.Range(0, 8).Select(_ => random.Next(10)));
        string number = "RSV-" + date.Year + "-" + reservationNumber;
        ReservationNumber = new(number);
    }
    public void SetHistory(ReservationHistory history)
    {
        _histories.Add(history);
    }
    public void SetPickUpForm(Form form)
    {
        PickUpForm = form;
    }
    public void SetDeliveryForm(Form form)
    {
        DeliveryForm = form;
    }

    #endregion
}
