using RentCarServer.Application.Behaviors;
using RentCarServer.Domain.ProtectionPackages;
using TS.MediatR;

namespace RentCarServer.Application.ProtectionPackages;
[Permission("protection_package:view")]
public sealed record ProtectionPackageGetAllQuery : IRequest<IQueryable<ProtectionPackageDto>>;

internal sealed class ProtectionPackageGetAllQueryHandler(
    IProtectionPackageRepository protectionPackageRepository) : IRequestHandler<ProtectionPackageGetAllQuery, IQueryable<ProtectionPackageDto>>
{
    public Task<IQueryable<ProtectionPackageDto>> Handle(ProtectionPackageGetAllQuery request, CancellationToken cancellationToken) =>
        Task.FromResult(protectionPackageRepository.GetAllWithAudit().MapTo());
}