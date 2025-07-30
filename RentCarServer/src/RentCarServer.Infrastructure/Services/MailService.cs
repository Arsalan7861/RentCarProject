using FluentEmail.Core;
using RentCarServer.Application.Services;

namespace RentCarServer.Infrastructure.Services;
internal class MailService(
    IFluentEmail fluentEmail) : IMailService
{
    public async Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        var sendResponse = await fluentEmail
            .To(to)
            .Subject(subject)
            .Body(body, isHtml: true)
            .SendAsync(cancellationToken);

        if (!sendResponse.Successful)
        {
            throw new InvalidOperationException(
                $"Mail gönderilemedi: {string.Join(", ", sendResponse.ErrorMessages)}");
        }
    }
}
