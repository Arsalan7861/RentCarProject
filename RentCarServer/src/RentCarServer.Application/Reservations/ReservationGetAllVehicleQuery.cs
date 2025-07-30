using Microsoft.EntityFrameworkCore;
using RentCarServer.Application.Vehicles;
using RentCarServer.Domain.Branches;
using RentCarServer.Domain.Categories;
using RentCarServer.Domain.Reservations;
using RentCarServer.Domain.Vehicles;
using TS.MediatR;
using TS.Result;

namespace RentCarServer.Application.Reservations;
public sealed record ReservationGetAllVehicleQuery(
    Guid BranchId,
    DateOnly PickUpDate,
    TimeOnly PickUpTime,
    DateOnly DeliveryDate,
    TimeOnly DeliveryTime
    ) : IRequest<Result<List<VehicleDto>>>;

internal sealed class ReservationGetAllVehicleQueryHandler(
    IReservationRepository reservationRepository,
    IVehicleRepository vehicleRepository,
    IBranchRepository branchRepository,
    ICategoryRepository categoryRepository
) : IRequestHandler<ReservationGetAllVehicleQuery, Result<List<VehicleDto>>>
{
    public async Task<Result<List<VehicleDto>>> Handle(ReservationGetAllVehicleQuery request, CancellationToken cancellationToken)
    {
        var pickUpDateTime = new DateTime(request.PickUpDate, request.PickUpTime);
        var deliveryDateTime = new DateTime(request.DeliveryDate, request.DeliveryTime);
        var unavailableVehicleIds = await reservationRepository
            .Where(r => r.PickUpLocationId == request.BranchId &&
                        r.PickUpDateTime.Value >= pickUpDateTime &&
                        r.DeliveryDateTime.Value.AddHours(1) <= deliveryDateTime)
            .Select(r => r.VehicleId.Value)
            .ToListAsync(cancellationToken);

        var vehicles = await vehicleRepository
            .GetAllWithAudit()
            .Where(p => !unavailableVehicleIds.Contains(p.Entity.Id) && p.Entity.BranchId.Value == request.BranchId)
            .MapTo(branchRepository.GetAll(), categoryRepository.GetAll())
            .ToListAsync(cancellationToken);

        return vehicles;
    }
}