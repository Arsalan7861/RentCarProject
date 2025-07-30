using RentCarServer.Domain.Abstractions;
using RentCarServer.Domain.Users;

namespace RentCarServer.Application;
internal static class ExtensionMethods
{
    public static IQueryable<EntityWithAuditDto<TEntity>> ApplyAuditDto<TEntity>(
        this IQueryable<TEntity> entities,
        IQueryable<User> users)
        where TEntity : Entity
    {
        var res = entities
            .Join(users,
                branch => branch.CreatedBy,
                user => user.Id,
                (branch, user) => new
                {
                    entity = branch,
                    createdUser = user
                })
            .GroupJoin(
                users,
                m => m.entity.UpdatedBy,
                m => m.Id,
                (b, user) => new
                { b.entity, b.createdUser, updatedUser = user })
            .SelectMany(s => s.updatedUser.DefaultIfEmpty(),
                (x, udatedUser) => new EntityWithAuditDto<TEntity>
                {
                    Entity = x.entity,
                    CreatedUser = x.createdUser,
                    UpdatedUser = udatedUser
                });
        return res;
    }
}


