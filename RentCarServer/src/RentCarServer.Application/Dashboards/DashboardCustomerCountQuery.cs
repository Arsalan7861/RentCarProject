using RentCarServer.Domain.Customers;
using TS.MediatR;
using TS.Result;

namespace RentCarServer.Application.Dashboards;
public sealed record DashboardCustomerCountQuery : IRequest<Result<int>>;

internal sealed class DashboardCustomerCountHandler(
    ICustomerRepository customerRepository
    ) : IRequestHandler<DashboardCustomerCountQuery, Result<int>>
{
    public async Task<Result<int>> Handle(DashboardCustomerCountQuery request, CancellationToken cancellationToken)
    {
        var count = await customerRepository
            .CountAsync(cancellationToken);
        return count;
    }
}
