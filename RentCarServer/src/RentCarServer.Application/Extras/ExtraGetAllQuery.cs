using RentCarServer.Application.Behaviors;
using RentCarServer.Domain.Extras;
using TS.MediatR;

namespace RentCarServer.Application.Extras;
[Permission("extra:view")]
public sealed record ExtraGetAllQuery : IRequest<IQueryable<ExtraDto>>;

internal sealed class ExtraGetAllQueryHandler(
    IExtraRepository extraRepository) : IRequestHandler<ExtraGetAllQuery, IQueryable<ExtraDto>>
{
    public Task<IQueryable<ExtraDto>> Handle(ExtraGetAllQuery request, CancellationToken cancellationToken) =>
        Task.FromResult(extraRepository.GetAllWithAudit().MapTo());
}