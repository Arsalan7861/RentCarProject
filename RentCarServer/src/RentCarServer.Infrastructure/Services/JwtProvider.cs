using GenericRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RentCarServer.Application.Services;
using RentCarServer.Domain.Branches;
using RentCarServer.Domain.LoginTokens;
using RentCarServer.Domain.LoginTokens.ValueObjects;
using RentCarServer.Domain.Roles;
using RentCarServer.Domain.Users;
using RentCarServer.Infrastructure.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace RentCarServer.Infrastructure.Services;
internal sealed class JwtProvider(
    ILoginTokenRepository loginTokenRepository,
    IRoleRepository roleRepository,
    IBranchRepository branchRepository,
    IUnitOfWork unitOfWork,
    IOptions<JwtOptions> options) : IJwtProvider
{
    public async Task<string> CreateTokenAsync(User user, CancellationToken cancellationToken = default)
    {
        var role = await roleRepository.FirstOrDefaultAsync(r => r.Id == user.RoleId, cancellationToken);
        var branch = await branchRepository.FirstOrDefaultAsync(b => b.Id == user.BranchId, cancellationToken);

        List<Claim> claims = new()
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim("fullName", user.FirstName.Value + " " + user.LastName.Value),
            new Claim("fullNameWithEmail", user.FullName.Value),
            new Claim("email", user.Email.Value),
            new Claim("role", role.Name.Value ?? string.Empty),
            new Claim("permissions", role is null ? "" : JsonSerializer.Serialize(role.Permissions.Select(r=>r.Value))),
            new Claim("branch", branch.Name.Value ?? string.Empty),
            new Claim("branchId", branch.Id ?? string.Empty)
        };

        SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(options.Value.SecretKey));
        SigningCredentials signingCredentials = new(securityKey, SecurityAlgorithms.HmacSha256);

        var expires = DateTime.Now.AddDays(1);
        JwtSecurityToken securityToken = new(
            issuer: options.Value.Issuer,
            audience: options.Value.Audience,
            claims: claims,
            notBefore: DateTime.Now,
            expires: expires,
            signingCredentials: signingCredentials);
        var handler = new JwtSecurityTokenHandler();
        var token = handler.WriteToken(securityToken);

        // yeni tokeni veritabanına ekle
        Token newToken = new(token);
        ExpiresDate expiresDate = new(expires);
        LoginToken loginToken = new(newToken, user.Id, expiresDate);
        loginTokenRepository.Add(loginToken);

        // eski tokenleri pasif hale getir
        var loginTokens = await loginTokenRepository
            .Where(x => x.UserId == user.Id && x.IsActive.Value == true)
            .ToListAsync(cancellationToken);
        foreach (var existingToken in loginTokens)
        {
            existingToken.SetIsActive(new(false));
        }
        loginTokenRepository.UpdateRange(loginTokens);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return token;
    }
}
