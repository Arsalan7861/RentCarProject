using Microsoft.EntityFrameworkCore;
using RentCarServer.Application.Services;
using RentCarServer.Domain.Reservations;
using TS.MediatR;
using TS.Result;

namespace RentCarServer.Application.Dashboards;

public sealed class ReservationWeeklyState
{
    public DateOnly Date { get; set; }
    public int TotalReservations { get; set; }
}

public sealed record DashboardWeeklyReservationStateQuery(
    DateTime CurrentDate
) : IRequest<Result<List<ReservationWeeklyState>>>;

internal sealed class DashboardWeeklyReservationStateQueryHandler(
    IReservationRepository reservationRepository,
    IClaimContext claimContext
) : IRequestHandler<DashboardWeeklyReservationStateQuery, Result<List<ReservationWeeklyState>>>
{
    public async Task<Result<List<ReservationWeeklyState>>> Handle(DashboardWeeklyReservationStateQuery request, CancellationToken cancellationToken)
    {
        var branchId = claimContext.GetBranchId();
        var today = new DateOnly(request.CurrentDate.Year, request.CurrentDate.Month, request.CurrentDate.Day);
        var startDate = today.AddDays(-6); // Last 7 days including today

        var reservations = await reservationRepository
            .Where(x => x.PickUpLocationId == branchId)
            .Where(x => x.PickUpDate.Value >= startDate &&
                        x.PickUpDate.Value <= today)
            .ToListAsync(cancellationToken);

        // Group by date
        var grouped = reservations
            .GroupBy(x => x.PickUpDate!.Value)
            .ToDictionary(g => g.Key, g => g.Count());

        // Fill all 7 days (even if 0 reservations)
        var weeklyState = Enumerable.Range(0, 7)
            .Select(i =>
            {
                var date = startDate.AddDays(i);
                grouped.TryGetValue(date, out int count);
                return new ReservationWeeklyState
                {
                    Date = date,
                    TotalReservations = count
                };
            })
            .ToList();

        return weeklyState;
    }
}
