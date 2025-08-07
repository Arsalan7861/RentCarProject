using GenericRepository;
using RentCarServer.Application.Services;
using RentCarServer.Domain.Users;
using RentCarServer.Domain.Users.ValueObjects;
using TS.MediatR;
using TS.Result;

namespace RentCarServer.Application.Auth;

public sealed record UpdateTFAStatusCommand : IRequest<Result<string>>;

public sealed class UpdateTFAStatusCommandHandler(
    IUserRepository userRepository,
    IClaimContext claimContext,
    IJwtProvider jwtProvider,
    IUnitOfWork unitOfWork
    ) : IRequestHandler<UpdateTFAStatusCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UpdateTFAStatusCommand request, CancellationToken cancellationToken)
    {
        var userId = claimContext.GetUserId();
        var user = await userRepository.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null)
        {
            return Result<string>.Failure("Kullanıcı bulunamadı");
        }



        // Update TFA status
        if (user.TFAStatus.Value)
        {
            user.SetTFAStatus(new TFAStatus(false));
        }
        else
        {
            user.SetTFAStatus(new TFAStatus(true));
        }

        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Generate new token with updated TFA status
        string newToken = await jwtProvider.CreateTokenAsync(user, cancellationToken);

        return newToken;
    }
}