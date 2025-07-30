namespace RentCarServer.Domain.Reservations.ValueObjects;

public sealed record PaymenyInformation(
    string CardNumber,
    string Owner);
