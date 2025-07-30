using GenericRepository;
using RentCarServer.Application.Behaviors;
using RentCarServer.Domain.Customers;
using TS.MediatR;
using TS.Result;

namespace RentCarServer.Application.Customers;
[Permission("customer:delete")]
public sealed record CustomerDeleteCommand(Guid Id) : IRequest<Result<string>>;

public sealed class CustomerDeleteCommandHandler(
    ICustomerRepository customerRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CustomerDeleteCommand, Result<string>>
{
    public async Task<Result<string>> Handle(CustomerDeleteCommand request, CancellationToken cancellationToken)
    {
        var customer = await customerRepository.FirstOrDefaultAsync(x => x.Id == request.Id);
        if (customer is null)
            return Result<string>.Failure("Müşteri bulunamadı.");

        customer.Delete();
        customerRepository.Update(customer);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return "Müşteri başarıyla silindi";
    }
}
