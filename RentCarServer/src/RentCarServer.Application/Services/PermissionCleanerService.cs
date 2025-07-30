using GenericRepository;
using Microsoft.EntityFrameworkCore;
using RentCarServer.Domain.Roles;

namespace RentCarServer.Application.Services;
public sealed class PermissionCleanerService(
    IRoleRepository roleRepository,
    PermissionService permissionService,
    IUnitOfWork unitOfWork)
{
    public async Task CleanRemovedPermissionsFromRoleAsync(CancellationToken cancellationToken = default)
    {
        var currentPermissions = permissionService.GetAll();
        var roles = await roleRepository.GetAllWithTracking()
            .ToListAsync(cancellationToken);

        foreach (var role in roles)
        {
            var currentPermissionsInRole = role.Permissions.Select(s => s.Value).ToList();
            var filteredPermissions = currentPermissionsInRole
                .Where(p => currentPermissions.Contains(p))
                .ToList();

            if (filteredPermissions.Count == currentPermissionsInRole.Count)
                continue;
            var permissions = filteredPermissions.Select(p => new Permission(p))
                .ToList();
            role.SetPermissions(permissions);
        }
        roleRepository.UpdateRange(roles);
        await unitOfWork.SaveChangesAsync();
    }
}
