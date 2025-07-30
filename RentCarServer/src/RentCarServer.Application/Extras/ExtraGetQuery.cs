using Microsoft.EntityFrameworkCore;
using RentCarServer.Application.Behaviors;
using RentCarServer.Domain.Extras;
using TS.MediatR;
using TS.Result;

namespace RentCarServer.Application.Extras;
[Permission("extra:view")]
public sealed record ExtraGetQuery(Guid Id) : IRequest<Result<ExtraDto>>;

internal sealed class ExtraGetQueryHandler(
    IExtraRepository extraRepository) : IRequestHandler<ExtraGetQuery, Result<ExtraDto>>
{
    public async Task<Result<ExtraDto>> Handle(ExtraGetQuery request, CancellationToken cancellationToken)
    {
        var res = await extraRepository
            .GetAllWithAudit()
            .MapTo()
            .Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (res is null)
            return Result<ExtraDto>.Failure("Ekstra bulunamad?.");

        return res;
    }
}