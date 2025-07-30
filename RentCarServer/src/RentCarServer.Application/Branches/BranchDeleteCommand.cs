using GenericRepository;
using RentCarServer.Application.Behaviors;
using RentCarServer.Domain.Branches;
using TS.MediatR;
using TS.Result;

namespace RentCarServer.Application.Branches;
[Permission("branch:delete")]
public sealed record BranchDeleteCommand(
    Guid Id) : IRequest<Result<string>>;

public sealed class BranchDeleteCommandHandler(
    IBranchRepository branchRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<BranchDeleteCommand, Result<string>>
{
    public async Task<Result<string>> Handle(BranchDeleteCommand request, CancellationToken cancellationToken)
    {
        var branch = await branchRepository.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (branch is null)
        {
            return Result<string>.Failure("Şube bulunamadı.");
        }

        branch.Delete();
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return "Şube başarıyla silindi.";
    }
}