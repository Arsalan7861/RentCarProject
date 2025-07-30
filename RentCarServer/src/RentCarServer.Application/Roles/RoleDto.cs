using RentCarServer.Domain.Abstractions;
using RentCarServer.Domain.Roles;

namespace RentCarServer.Application.Roles;
public sealed class RoleDto : EntityDto
{
    public string Name { get; set; } = default!;
    public int PermissionCount { get; set; }
    public List<string> Permissions { get; set; } = new();
}

public static class RoleExtensions
{
    public static IQueryable<RoleDto> MapTo(this IQueryable<EntityWithAuditDto<Role>> entities)
    {
        var res = entities.Select(x => new RoleDto
        {
            Id = x.Entity.Id,
            Name = x.Entity.Name.Value,
            PermissionCount = x.Entity.Permissions.Count,
            IsActive = x.Entity.IsActive,
            CreatedAt = x.Entity.CreatedAt,
            CreatedBy = x.Entity.CreatedBy,
            CreatedFullName = x.CreatedUser.FullName.Value,
            UpdatedAt = x.Entity.UpdatedAt,
            UpdatedBy = x.Entity.UpdatedBy == null ? null : x.Entity.UpdatedBy.Value,
            UpdatedFullName = x.UpdatedUser == null ? null : x.UpdatedUser.FullName.Value
        }).AsQueryable();

        return res;
    }
    public static IQueryable<RoleDto> MapToGet(this IQueryable<EntityWithAuditDto<Role>> entities)
    {
        var res = entities.Select(x => new RoleDto
        {
            Id = x.Entity.Id,
            Name = x.Entity.Name.Value,
            PermissionCount = x.Entity.Permissions.Count,
            Permissions = x.Entity.Permissions.Select(s=>s.Value).ToList(),
            IsActive = x.Entity.IsActive,
            CreatedAt = x.Entity.CreatedAt,
            CreatedBy = x.Entity.CreatedBy,
            CreatedFullName = x.CreatedUser.FullName.Value,
            UpdatedAt = x.Entity.UpdatedAt,
            UpdatedBy = x.Entity.UpdatedBy == null ? null : x.Entity.UpdatedBy.Value,
            UpdatedFullName = x.UpdatedUser == null ? null : x.UpdatedUser.FullName.Value
        }).AsQueryable();

        return res;
    }
}
