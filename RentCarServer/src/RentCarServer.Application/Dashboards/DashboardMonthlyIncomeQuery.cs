using Microsoft.EntityFrameworkCore;
using RentCarServer.Application.Services;
using RentCarServer.Domain.Reservations;
using RentCarServer.Domain.Reservations.ValueObjects;
using TS.MediatR;
using TS.Result;

namespace RentCarServer.Application.Dashboards;
public sealed record DashboardMonthlyIncomeQuery(
    DateTime CurrentDate) : IRequest<Result<decimal>>;

internal sealed class DashboardMonthlyIncomeHandler(
    IReservationRepository reservationRepository,
    IClaimContext claimContext
    ) : IRequestHandler<DashboardMonthlyIncomeQuery, Result<decimal>>
{
    public async Task<Result<decimal>> Handle(DashboardMonthlyIncomeQuery request, CancellationToken cancellationToken)
    {
        var branchId = claimContext.GetBranchId();
        var startDate = new DateOnly(request.CurrentDate.Year, request.CurrentDate.Month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);
        var totalPrice = await reservationRepository
            .Where(x => x.PickUpLocationId == branchId)
            .Where(x => x.PickUpDate.Value >= startDate && x.PickUpDate.Value <= endDate)
            .Where(x => x.Status.Value == Status.Completed.Value)
            .SumAsync(x => x.Total.Value, cancellationToken);

        return totalPrice;
    }
}
