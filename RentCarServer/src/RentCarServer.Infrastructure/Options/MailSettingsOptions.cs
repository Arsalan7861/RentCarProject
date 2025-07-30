namespace RentCarServer.Infrastructure.Options;
internal sealed class MailSettingsOptions
{
    public string Email { get; set; } = default!;
    public string Smtp { get; set; } = default!;
    public int Port { get; set; }
    public string SSL { get; set; }
    public string UserId { get; set; } = default!;
    public string Password { get; set; } = default!;
}
