using Microsoft.EntityFrameworkCore;
using RentCarServer.Application.Services;
using RentCarServer.Domain.Vehicles;
using TS.MediatR;
using TS.Result;

namespace RentCarServer.Application.Dashboards;
public sealed record DashboardVehicleCountQuery : IRequest<Result<int>>;

internal sealed class DashboardVehicleCountHandler(
    IVehicleRepository vehicleRepository,
    IClaimContext claimContext
    ) : IRequestHandler<DashboardVehicleCountQuery, Result<int>>
{
    public async Task<Result<int>> Handle(DashboardVehicleCountQuery request, CancellationToken cancellationToken)
    {
        var branchId = claimContext.GetBranchId();
        var count = await vehicleRepository
            .Where(x => x.BranchId.Value == branchId)
            .CountAsync(cancellationToken);

        return count;
    }
}