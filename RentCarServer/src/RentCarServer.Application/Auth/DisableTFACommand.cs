using RentCarServer.Application.Services;
using RentCarServer.Domain.Users;
using RentCarServer.Domain.Users.ValueObjects;
using TS.MediatR;
using TS.Result;

namespace RentCarServer.Application.Auth;

public sealed record DisableTFACommand : IRequest<Result<string>>;

public sealed class DisableTFACommandHandler(
    IUserRepository userRepository,
    IClaimContext claimContext,
    IJwtProvider jwtProvider) : IRequestHandler<DisableTFACommand, Result<string>>
{
    public async Task<Result<string>> Handle(DisableTFACommand request, CancellationToken cancellationToken)
    {
        var userId = claimContext.GetUserId();
        var user = await userRepository.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null)
        {
            return Result<string>.Failure("User not found");
        }

        user.SetTFAStatus(new TFAStatus(false));
        userRepository.Update(user);

        // Generate new token with updated TFA status
        string newToken = await jwtProvider.CreateTokenAsync(user, cancellationToken);

        return newToken;
    }
}