using RentCarServer.Domain.Abstractions;
using RentCarServer.Domain.ProtectionPackages.ValueObjects;
using RentCarServer.Domain.Shared;

namespace RentCarServer.Domain.ProtectionPackages;
public sealed class ProtectionPackage : Entity, IAggregate
{
    private readonly List<Coverage> _coverages = new();

    public ProtectionPackage() { }

    public ProtectionPackage(
        Name name,
        Price price,
        IsRecommended isRecommended,
        OrderNumber orderNumber,
        IEnumerable<Coverage> coverages,
        bool isActive)
    {
        SetName(name);
        SetPrice(price);
        SetIsRecommended(isRecommended);
        SetCoverages(coverages);
        SetStatus(isActive);
        SetOrderNumber(orderNumber);
    }

    public Name Name { get; private set; } = default!;
    public Price Price { get; private set; } = default!;
    public IsRecommended IsRecommended { get; private set; } = default!;
    public OrderNumber OrderNumber { get; private set; } = default!;
    public IReadOnlyCollection<Coverage> Coverages => _coverages;

    public void SetName(Name name) => Name = name;
    public void SetPrice(Price price) => Price = price;
    public void SetIsRecommended(IsRecommended isRecommended) => IsRecommended = isRecommended;
    public void SetOrderNumber(OrderNumber orderNumber) => OrderNumber = orderNumber;
    public void SetCoverages(IEnumerable<Coverage> coverages)
    {
        _coverages.Clear();
        _coverages.AddRange(coverages);
    }
}