using RentCarServer.Domain.Abstractions;
using RentCarServer.Domain.LoginTokens.ValueObjects;

namespace RentCarServer.Domain.LoginTokens;
public sealed class LoginToken
{
    private LoginToken() { }
    public LoginToken(
        Token token,
        IdentityId userId,
        ExpiresDate expiresDate)
    {
        Id = new(Guid.CreateVersion7());
        SetToken(token);
        SetUserId(userId);
        SetIsActive(new(true));
        SetExpiredDate(expiresDate);
    }

    public IdentityId Id { get; private set; }
    public IsActive IsActive { get; private set; } = default!;
    public Token Token { get; private set; } = default!;
    public IdentityId UserId { get; private set; } = default!;
    public ExpiresDate ExpiresDate { get; private set; } = default!;

    #region Behaviors

    public void SetToken(Token token)
    {
        Token = token;
    }

    public void SetUserId(IdentityId userId)
    {
        UserId = userId;
    }
    public void SetIsActive(IsActive isActive)
    {
        IsActive = isActive;
    }
    public void SetExpiredDate(ExpiresDate expiredDate)
    {
        ExpiresDate = expiredDate;
    }
    #endregion
}
