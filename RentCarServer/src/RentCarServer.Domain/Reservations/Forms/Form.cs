using RentCarServer.Domain.Reservations.Forms.ValueObjects;
using RentCarServer.Domain.Reservations.ValueObjects;
using RentCarServer.Domain.Shared;
using RentCarServer.Domain.Vehicles.ValueObjects;

namespace RentCarServer.Domain.Reservations.Forms;
public sealed record Form
{
    private readonly List<Supply> _supplies = new();
    private readonly List<ImageUrl> _imageUrls = new();
    private readonly List<Damage> _damages = new();

    public Form()
    {
    }

    public Form(
        Kilometer kilometer,
        List<Supply> supplies,
        List<ImageUrl> imageUrls,
        List<Damage> damages,
        Note note)
    {
        SetKilometer(kilometer);
        SetSupplies(supplies);
        SetImageUrls(imageUrls);
        SetDamages(damages);
        SetNote(note);
    }

    public Kilometer Kilometer { get; private set; } = default!;
    public IReadOnlyCollection<Supply> Supplies => _supplies;
    public IReadOnlyCollection<ImageUrl> ImageUrls => _imageUrls;
    public IReadOnlyCollection<Damage> Damages => _damages;
    public Note Note { get; private set; } = default!;
    #region Behaviors

    public void SetKilometer(Kilometer kilometer)
    {
        Kilometer = kilometer;
    }
    public void SetSupplies(List<Supply> supply)
    {
        _supplies.Clear();
        _supplies.AddRange(supply);
    }
    public void SetImageUrls(List<ImageUrl> imageUrl)
    {
        _imageUrls.Clear();
        _imageUrls.AddRange(imageUrl);
    }
    public void SetDamages(List<Damage> damage)
    {
        _damages.Clear();
        _damages.AddRange(damage);
    }
    public void SetNote(Note note)
    {
        Note = note;
    }

    #endregion
}
