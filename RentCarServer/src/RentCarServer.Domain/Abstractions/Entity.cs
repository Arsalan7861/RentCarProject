using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using RentCarServer.Domain.Users;

namespace RentCarServer.Domain.Abstractions;

public abstract class Entity // abstract olması Entity sınıfının var olamamsı ve diğer sınıfların bu sınıftan inherit etmesi anlamına gelir.
{
    protected Entity()
    {
        Id = new IdentityId(Guid.CreateVersion7());
        IsActive = true;
    }

    public IdentityId Id { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; } // DateTimeOffset, tarih ve saat bilgisini UTC olarak saklar, yani database'de +3'i de saklayabiliriz.
    public IdentityId CreatedBy { get; private set; } = default!; // default! kullanarak null referans hatasını önlüyoruz, çünkü CreatedBy her zaman bir değer alacak.
    public DateTimeOffset? UpdatedAt { get; private set; }
    public IdentityId? UpdatedBy { get; private set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; private set; }
    public IdentityId? DeletedBy { get; private set; }

    public void SetStatus(bool isActive)
    {
        IsActive = isActive;
    }

    public void Delete()
    {
        IsDeleted = true;
    }
}
public sealed record IdentityId(Guid Value) // seealed : sınıfın başka sınıflar tarafından inherit edilmesini engeller. record : immutable bir sınıf oluşturur.
{
    public static implicit operator Guid(IdentityId id) => id.Value;
    public static implicit operator string(IdentityId id) => id.Value.ToString();
}


