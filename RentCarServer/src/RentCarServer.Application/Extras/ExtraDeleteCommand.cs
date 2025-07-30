using GenericRepository;
using RentCarServer.Application.Behaviors;
using RentCarServer.Domain.Extras;
using TS.MediatR;
using TS.Result;

namespace RentCarServer.Application.Extras;
[Permission("extra:delete")]
public sealed record ExtraDeleteCommand(Guid Id) : IRequest<Result<string>>;

internal sealed class ExtraDeleteCommandHandler(
    IExtraRepository extraRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<ExtraDeleteCommand, Result<string>>
{
    public async Task<Result<string>> Handle(ExtraDeleteCommand request, CancellationToken cancellationToken)
    {
        var extra = await extraRepository.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (extra is null)
            return Result<string>.Failure("Ekstra bulunamad?.");

        extra.Delete();
        extraRepository.Update(extra);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return "Ekstra ba?ar?yla silindi.";
    }
}