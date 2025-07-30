namespace RentCarServer.Domain.Abstractions;

public abstract class EntityDto // abstract olması Entity sınıfının var olamamsı ve diğer sınıfların bu sınıftan inherit etmesi anlamına gelir.
{
    public Guid Id { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; } // DateTimeOffset, tarih ve saat bilgisini UTC olarak saklar, yani database'de +3'i de saklayabiliriz.
    public Guid CreatedBy { get; set; }
    public string CreatedFullName { get; set; } = default!;
    public DateTimeOffset? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public string? UpdatedFullName { get; set; }
}


