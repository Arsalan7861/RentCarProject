using RentCarServer.Domain.Abstractions;
using RentCarServer.Domain.Categories;

namespace RentCarServer.Application.Categories;
public sealed class CategoryDto : EntityDto
{
    public string Name { get; set; } = default!;
}

public static class CategoryExtensions
{
    public static IQueryable<CategoryDto> MapTo(this IQueryable<EntityWithAuditDto<Category>> entities)
    {
        return entities.Select(x => new CategoryDto
        {
            Id = x.Entity.Id,
            Name = x.Entity.Name.Value,
            IsActive = x.Entity.IsActive,
            CreatedAt = x.Entity.CreatedAt,
            CreatedBy = x.Entity.CreatedBy,
            CreatedFullName = x.CreatedUser.FullName.Value,
            UpdatedAt = x.Entity.UpdatedAt,
            UpdatedBy = x.Entity.UpdatedBy == null ? null : x.Entity.UpdatedBy.Value,
            UpdatedFullName = x.UpdatedUser == null ? null : x.UpdatedUser.FullName.Value
        }).AsQueryable();
    }
}