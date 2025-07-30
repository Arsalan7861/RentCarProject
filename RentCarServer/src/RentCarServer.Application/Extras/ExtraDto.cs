using RentCarServer.Domain.Abstractions;
using RentCarServer.Domain.Extras;

namespace RentCarServer.Application.Extras;
public sealed class ExtraDto : EntityDto
{
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
    public string Description { get; set; } = default!;
}

public static class ExtraExtensions
{
    public static IQueryable<ExtraDto> MapTo(this IQueryable<EntityWithAuditDto<Extra>> entities)
    {
        return entities.Select(x => new ExtraDto
        {
            Id = x.Entity.Id,
            Name = x.Entity.Name.Value,
            Price = x.Entity.Price.Value,
            Description = x.Entity.Description.Value,
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