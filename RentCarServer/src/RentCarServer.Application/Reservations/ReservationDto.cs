using RentCarServer.Domain.Abstractions;
using RentCarServer.Domain.Branches;
using RentCarServer.Domain.Categories;
using RentCarServer.Domain.Customers;
using RentCarServer.Domain.Extras;
using RentCarServer.Domain.ProtectionPackages;
using RentCarServer.Domain.Reservations;
using RentCarServer.Domain.Reservations.ValueObjects;
using RentCarServer.Domain.Vehicles;

namespace RentCarServer.Application.Reservations;

public sealed class ReservationPickUpDto
{
    public string Name { get; set; } = default!;
    public string FullAddress { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
}
public sealed class ReservationCustomerDto
{
    public string FullName { get; set; } = default!;
    public string IdentityNumber { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string FullAddress { get; set; } = default!;
}
public sealed class ReservationVehicleDto
{
    public Guid Id { get; set; }
    public string Brand { get; set; } = default!;
    public string Model { get; set; } = default!;
    public int ModelYear { get; set; } = default!;
    public string Color { get; set; } = default!;
    public string CategoryName { get; set; } = default!;
    public decimal FuelConsumption { get; set; } = default!;
    public string FuelType { get; set; } = default!;
    public int SeatCount { get; set; } = default!;
    public string TractionType { get; set; } = default!;
    public int Kilometer { get; set; } = default!;
    public string ImageUrl { get; set; } = default!;
    public string Plate { get; set; } = default!;
}
public sealed class ReservationExtraDto
{
    public Guid ExtraId { get; set; }
    public string ExtraName { get; set; } = default!;
    public decimal ExtraPrice { get; set; } = default!;
}
public sealed class ReservationDto : EntityDto
{
    public string ReservationNumber { get; set; } = default!;
    public Guid CustomerId { get; set; }
    public ReservationCustomerDto Customer { get; set; } = default!;
    public Guid PickUpLocationId { get; set; }
    public ReservationPickUpDto PickUp { get; set; } = default!;
    public DateOnly PickUpDate { get; set; } = default!;
    public TimeOnly PickUpTime { get; set; } = default!;
    public DateTimeOffset PickUpDateTime { get; set; } = default!;
    public DateOnly DeliveryDate { get; set; } = default!;
    public TimeOnly DeliveryTime { get; set; } = default!;
    public DateTimeOffset DeliveryDateTime { get; set; } = default!;
    public Guid VehicleId { get; set; }
    public decimal VehicleDailyPrice { get; set; } = default!;
    public ReservationVehicleDto Vehicle { get; set; } = default!;
    public Guid ProtectionPackageId { get; set; }
    public decimal ProtectionPackagePrice { get; set; } = default!;
    public string ProtectionPackageName { get; set; } = default!;
    public List<ReservationExtraDto> ReservationExtras { get; set; } = default!;
    public string Note { get; set; } = default!;
    public decimal Total { get; set; } = default!;
    public string Status { get; set; } = default!;
    public int TotalDay { get; set; } = default!;
    public PaymenyInformation PaymentInformation { get; set; } = default!;
    public List<ReservationHistory> Histories { get; set; } = default!;
}


public static class Extensions
{
    public static IQueryable<ReservationDto> MapTo(
        this IQueryable<EntityWithAuditDto<Reservation>> entities,
             IQueryable<Customer> customers,
             IQueryable<Branch> branches,
             IQueryable<Vehicle> vehicles,
             IQueryable<Category> categories,
             IQueryable<ProtectionPackage> protectionPackages,
             IQueryable<Extra> extras
        )
    {
        var res = entities
            .Join(customers, m => m.Entity.CustomerId, e => e.Id, (r, customer) => new
            {
                r.Entity,
                r.CreatedUser,
                r.UpdatedUser,
                Customer = customer
            })
            .Join(branches, m => m.Entity.PickUpLocationId, e => e.Id, (r, branch) => new
            {
                r.Entity,
                r.CreatedUser,
                r.UpdatedUser,
                r.Customer,
                Branch = branch
            })
            .Join(protectionPackages, m => m.Entity.ProtectionPackageId, e => e.Id, (r, protectionPackage) => new
            {
                r.Entity,
                r.CreatedUser,
                r.UpdatedUser,
                r.Customer,
                r.Branch,
                ProtectionPackage = protectionPackage
            })
            .Join(vehicles, m => m.Entity.VehicleId, m => m.Id, (r, vehicle) => new
            {
                r.Entity,
                r.CreatedUser,
                r.UpdatedUser,
                r.Customer,
                r.Branch,
                r.ProtectionPackage,
                Vehicle = vehicle
            })
            .Select(e => new ReservationDto
            {
                ReservationNumber = e.Entity.ReservationNumber.Value,
                Id = e.Entity.Id,
                CustomerId = e.Entity.CustomerId,
                Customer = new ReservationCustomerDto
                {
                    FullName = e.Customer.FullName.Value,
                    IdentityNumber = e.Customer.IdentityNumber.Value,
                    PhoneNumber = e.Customer.PhoneNumber.Value,
                    Email = e.Customer.Email.Value,
                    FullAddress = e.Customer.FullAddress.Value
                },
                PickUpLocationId = e.Entity.PickUpLocationId,
                PickUp = new ReservationPickUpDto
                {
                    Name = e.Branch.Name.Value,
                    FullAddress = e.Branch.Address.FullAddress,
                    PhoneNumber = e.Branch.Contact.PhoneNumber1
                },
                PickUpDate = e.Entity.PickUpDate.Value,
                PickUpTime = e.Entity.PickUpTime.Value,
                PickUpDateTime = e.Entity.PickUpDateTime.Value,
                DeliveryDate = e.Entity.DeliveryDate.Value,
                DeliveryTime = e.Entity.DeliveryTime.Value,
                DeliveryDateTime = e.Entity.DeliveryDateTime.Value,
                VehicleId = e.Entity.VehicleId,
                VehicleDailyPrice = e.Entity.VehicleDailyPrice.Value,
                Vehicle = new ReservationVehicleDto
                {
                    Id = e.Vehicle.Id,
                    Brand = e.Vehicle.Brand.Value,
                    Model = e.Vehicle.Model.Value,
                    ModelYear = e.Vehicle.ModelYear.Value,
                    CategoryName = categories.First(i => i.Id == e.Vehicle.CategoryId.Value).Name.Value,
                    Color = e.Vehicle.Color.Value,
                    FuelConsumption = e.Vehicle.FuelConsumption.Value,
                    FuelType = e.Vehicle.FuelType.Value,
                    SeatCount = e.Vehicle.SeatCount.Value,
                    TractionType = e.Vehicle.TractionType.Value,
                    Kilometer = e.Vehicle.Kilometer.Value,
                    ImageUrl = e.Vehicle.ImageUrl.Value,
                    Plate = e.Vehicle.Plate.Value
                },
                ProtectionPackageId = e.Entity.ProtectionPackageId,
                ProtectionPackagePrice = e.Entity.ProtectionPackagePrice.Value,
                ProtectionPackageName = e.ProtectionPackage.Name.Value,
                ReservationExtras = e.Entity.ReservationExtras.Join(extras, e => e.ExtraId, extra => extra.Id, (e, extra) => new ReservationExtraDto
                {
                    ExtraId = e.ExtraId,
                    ExtraName = extra.Name.Value,
                    ExtraPrice = extra.Price.Value
                }).ToList(),
                Note = e.Entity.Note.Value,
                Total = e.Entity.Total.Value,
                Status = e.Entity.Status.Value,
                TotalDay = e.Entity.TotalDay.Value,
                PaymentInformation = e.Entity.PaymentInformation,
                Histories = e.Entity.Histories.ToList(),
                IsActive = e.Entity.IsActive,
                CreatedAt = e.Entity.CreatedAt,
                CreatedBy = e.Entity.CreatedBy.Value,
                CreatedFullName = e.CreatedUser.FullName.Value,
                UpdatedAt = e.Entity.UpdatedAt,
                UpdatedBy = e.Entity.UpdatedBy == null ? null : e.Entity.UpdatedBy.Value,
                UpdatedFullName = e.UpdatedUser == null ? null : e.UpdatedUser.FullName.Value
            });

        return res;
    }
}