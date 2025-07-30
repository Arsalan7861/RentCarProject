using Microsoft.EntityFrameworkCore;
using RentCarServer.Application.Behaviors;
using RentCarServer.Domain.Customers;
using TS.MediatR;
using TS.Result;

namespace RentCarServer.Application.Customers;
[Permission("customer:view")]
public sealed record CustomerGetQuery(Guid Id) : IRequest<Result<CustomerDto>>;

public sealed class CustomerGetQueryHandler(
    ICustomerRepository customerRepository) : IRequestHandler<CustomerGetQuery, Result<CustomerDto>>
{
    public async Task<Result<CustomerDto>> Handle(CustomerGetQuery request, CancellationToken cancellationToken)
    {
        var res = await customerRepository
            .GetAllWithAudit()
            .MapTo()
            .Where(p => p.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (res is null)
            return Result<CustomerDto>.Failure("Müşteri bulunamadı.");

        return res;
    }
}
