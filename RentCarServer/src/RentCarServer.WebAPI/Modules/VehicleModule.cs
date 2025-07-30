using Microsoft.AspNetCore.Mvc;
using RentCarServer.Application.Vehicles;
using TS.MediatR;
using TS.Result;

namespace RentCarServer.WebAPI.Modules;

public static class VehicleModule
{
    public static void MapVehicle(this IEndpointRouteBuilder builder)
    {
        var app = builder
            .MapGroup("/vehicles")
            .WithTags("Vehicles")
            .RequireRateLimiting("fixed")
            .RequireAuthorization();

        // Create
        app.MapPost(string.Empty,
            async ([FromForm] VehicleCreateCommand request, ISender sender, CancellationToken cancellationToken) =>
            {
                var res = await sender.Send(request, cancellationToken);
                return res.IsSuccessful
                    ? Results.Ok(res)
                    : Results.InternalServerError(res);
            })
            .Accepts<VehicleCreateCommand>("multipart/form-data")
            .Produces<Result<string>>()
            .DisableAntiforgery();

        // Update
        app.MapPut(string.Empty,
            async ([FromForm] VehicleUpdateCommand request, ISender sender, CancellationToken cancellationToken) =>
            {
                var res = await sender.Send(request, cancellationToken);
                return res.IsSuccessful
                    ? Results.Ok(res)
                    : Results.InternalServerError(res);
            })
            .Accepts<VehicleUpdateCommand>("multipart/form-data")
            .Produces<Result<string>>()
            .DisableAntiforgery();

        // Delete
        app.MapDelete("{id}",
            async (Guid id, ISender sender, CancellationToken cancellationToken) =>
            {
                var res = await sender.Send(new VehicleDeleteCommand(id), cancellationToken);
                return res.IsSuccessful
                    ? Results.Ok(res)
                    : Results.InternalServerError(res);
            })
            .Produces<Result<string>>();

        // Get by Id
        app.MapGet("{id}",
            async (Guid id, ISender sender, CancellationToken cancellationToken) =>
            {
                var res = await sender.Send(new VehicleGetQuery(id), cancellationToken);
                return res.IsSuccessful
                    ? Results.Ok(res)
                    : Results.NotFound(res);
            })
            .Produces<Result<VehicleDto>>();
    }
}