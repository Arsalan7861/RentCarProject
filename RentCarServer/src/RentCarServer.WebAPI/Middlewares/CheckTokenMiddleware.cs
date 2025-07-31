
using RentCarServer.Domain.LoginTokens;
using System.Security.Claims;

namespace RentCarServer.WebAPI.Middlewares;

public sealed class CheckTokenMiddleware(
    ILoginTokenRepository loginTokenRepository) : IMiddleware
{
    public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
    {
        try
        {
            var token = httpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", string.Empty);
            if (string.IsNullOrWhiteSpace(token))
            {
                await next(httpContext);
                return;
            }

            var userId = httpContext.User.Claims
                .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                throw new TokenException("Kullanıcı kimliği bulunamadı.");
            }

            var isTokenAvailable = await loginTokenRepository.AnyAsync(
                x => x.Token.Value == token
                && x.UserId == userId
                && x.IsActive.Value == true);
            if (!isTokenAvailable)
            {
                throw new TokenException("Geçersiz veya süresi dolmuş token.");
            }

            await next(httpContext);
        }
        catch (Exception)
        {
            throw;
        }
    }
}

public sealed class TokenException : Exception
{
    public TokenException(string message) : base(message)
    {
    }
}
