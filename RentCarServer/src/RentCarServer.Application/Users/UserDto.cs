using RentCarServer.Domain.Abstractions;
using RentCarServer.Domain.Branches;
using RentCarServer.Domain.Roles;
using RentCarServer.Domain.Users;

namespace RentCarServer.Application.Users;
public sealed class UserDto : EntityDto
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = default!;
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = default!;
}

public static class UserExtensions
{
    public static IQueryable<UserDto> Mapto(
        this IQueryable<EntityWithAuditDto<User>> entities,
        IQueryable<Role> roles,
        IQueryable<Branch> branches)
    {
        var res = entities
            .Join(roles, m => m.Entity.RoleId, m => m.Id, (e, role)
            => new { e.Entity, e.CreatedUser, e.UpdatedUser, Role = role })
            .Join(branches, m => m.Entity.BranchId, m => m.Id, (entity, branch)
            => new { entity.Entity, entity.CreatedUser, entity.UpdatedUser, entity.Role, Branch = branch })
            .Select(x => new UserDto
            {
                Id = x.Entity.Id,
                FirstName = x.Entity.FirstName.Value,
                LastName = x.Entity.LastName.Value,
                FullName = x.Entity.FullName.Value,
                Email = x.Entity.Email.Value,
                UserName = x.Entity.UserName.Value,
                BranchId = x.Entity.BranchId,
                BranchName = x.Branch.Name.Value,
                RoleId = x.Entity.RoleId,
                RoleName = x.Role.Name.Value,
                IsActive = x.Entity.IsActive,
                CreatedAt = x.Entity.CreatedAt,
                CreatedBy = x.Entity.CreatedBy.Value,
                CreatedFullName = x.CreatedUser.FullName.Value,
                UpdatedAt = x.Entity.UpdatedAt,
                UpdatedBy = x.Entity.UpdatedBy != null ? x.Entity.UpdatedBy.Value : null,
                UpdatedFullName = x.UpdatedUser != null ? x.UpdatedUser.FullName.Value : null
            });

        return res;
    }
}
