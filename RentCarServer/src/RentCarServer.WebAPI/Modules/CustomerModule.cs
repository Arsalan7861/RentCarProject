﻿using RentCarServer.Application.Customers;
using TS.MediatR;
using TS.Result;

namespace RentCarServer.WebAPI.Modules;

public static class CustomerModule
{
    public static void MapCustomer(this IEndpointRouteBuilder builder)
    {
        var app = builder
            .MapGroup("/customers")
            .WithTags("Customers")
            .RequireRateLimiting("fixed")
            .RequireAuthorization();

        app.MapPost(string.Empty,
                async (CustomerCreateCommand request, ISender sender, CancellationToken cancellationToken) =>
                {
                    var res = await sender.Send(request, cancellationToken);
                    return res.IsSuccessful
                        ? Results.Ok(res)
                        : Results.InternalServerError(res);
                })
            .Produces<Result<string>>();
        app.MapPut(string.Empty,
                async (CustomerUpdateCommand request, ISender sender, CancellationToken cancellationToken) =>
                {
                    var res = await sender.Send(request, cancellationToken);
                    return res.IsSuccessful
                        ? Results.Ok(res)
                        : Results.InternalServerError(res);
                })
            .Produces<Result<string>>();
        app.MapDelete("{id}",
                async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                {
                    var res = await sender.Send(new CustomerDeleteCommand(id), cancellationToken);
                    return res.IsSuccessful
                        ? Results.Ok(res)
                        : Results.InternalServerError(res);
                })
            .Produces<Result<string>>();
        app.MapGet("{id}",
                async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                {
                    var res = await sender.Send(new CustomerGetQuery(id), cancellationToken);
                    return res.IsSuccessful
                        ? Results.Ok(res)
                        : Results.InternalServerError(res);
                })
            .Produces<Result<CustomerDto>>();
    }
}
