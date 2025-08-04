using RentCarServer.Application.Dashboards;
using TS.MediatR;
using TS.Result;

namespace RentCarServer.WebAPI.Modules;

public static class DashboardModule
{
    public static void MapDashboard(this IEndpointRouteBuilder builder)
    {
        var app = builder
            .MapGroup("/dashboard")
            .RequireRateLimiting("fixed")
            .WithTags("Dashboard")
            .RequireAuthorization();

        app.MapGet("active-reservation-count",
            async (ISender sender, CancellationToken cancelationToken) =>
            {
                var res = await sender.Send(new DashboardActiveReservationCountQuery(), cancelationToken);
                return res.IsSuccessful ? Results.Ok(res)
                    : Results.InternalServerError(res);
            })
            .Produces<Result<int>>();

        app.MapGet("vehicle-count",
            async (ISender sender, CancellationToken cancelationToken) =>
            {
                var res = await sender.Send(new DashboardVehicleCountQuery(), cancelationToken);
                return res.IsSuccessful ? Results.Ok(res)
                    : Results.InternalServerError(res);
            })
            .Produces<Result<int>>();

        app.MapGet("monthly-income",
            async (ISender sender, CancellationToken cancelationToken) =>
            {
                var res = await sender.Send(new DashboardMonthlyIncomeQuery(DateTime.Now), cancelationToken);
                return res.IsSuccessful ? Results.Ok(res)
                    : Results.InternalServerError(res);
            })
            .Produces<Result<decimal>>();

        app.MapGet("customer-count",
            async (ISender sender, CancellationToken cancelationToken) =>
            {
                var res = await sender.Send(new DashboardCustomerCountQuery(), cancelationToken);
                return res.IsSuccessful ? Results.Ok(res)
                    : Results.InternalServerError(res);
            })
            .Produces<Result<int>>();
        app.MapGet("weekly-reservation-state",
            async (ISender sender, CancellationToken cancelationToken) =>
            {
                var res = await sender.Send(new DashboardWeeklyReservationStateQuery(DateTime.Now), cancelationToken);
                return res.IsSuccessful ? Results.Ok(res)
                    : Results.InternalServerError(res);
            })
            .Produces<Result<List<ReservationWeeklyState>>>();
    }
}
