using RentCarServer.Application.Behaviors;
using RentCarServer.Domain.Customers;
using TS.MediatR;

namespace RentCarServer.Application.Customers;
[Permission("customer:view")]
public sealed record CustomerGetAllQuery : IRequest<IQueryable<CustomerDto>>;

public sealed class CustomerGetAllQueryHandler(
    ICustomerRepository customerRepository) : IRequestHandler<CustomerGetAllQuery, IQueryable<CustomerDto>>
{
    public Task<IQueryable<CustomerDto>> Handle(CustomerGetAllQuery request, CancellationToken cancellationToken)
    {
        var res = customerRepository
            .GetAllWithAudit()
            .MapTo()
            .AsQueryable();
        return Task.FromResult(res);
    }
}