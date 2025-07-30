using RentCarServer.Application.Behaviors;
using RentCarServer.Application.Services;
using TS.MediatR;
using TS.Result;

namespace RentCarServer.Application.Permissions;
public sealed class PermissionGetAllQuery : IRequest<Result<List<string>>>;
[Permission("permission:view")]

internal sealed class PermissionGetAllQueryHandler(
    PermissionService permissionService) : IRequestHandler<PermissionGetAllQuery, Result<List<string>>>
{
    public Task<Result<List<string>>> Handle(PermissionGetAllQuery request, CancellationToken cancellationToken)
        => Task.FromResult(
            Result<List<string>>.Succeed(permissionService.GetAll())
        );
}
