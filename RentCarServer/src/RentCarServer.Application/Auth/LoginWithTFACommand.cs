using GenericRepository;
using RentCarServer.Application.Services;
using RentCarServer.Domain.Users;
using TS.MediatR;
using TS.Result;

namespace RentCarServer.Application.Auth;
public sealed record LoginWithTFACommand(
    string EmailOrUsername,
    string TFACode,
    string TFAConfirmCode) : IRequest<Result<LogincommandResponse>>;

internal sealed class LoginWithTFACommandHandler(
    IUserRepository userRepository,
    IJwtProvider jwtProvider,
    IUnitOfWork unitOfWork) : IRequestHandler<LoginWithTFACommand, Result<LogincommandResponse>>
{
    public async Task<Result<LogincommandResponse>> Handle(LoginWithTFACommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.FirstOrDefaultAsync(
            x => x.UserName.Value == request.EmailOrUsername || x.Email.Value == request.EmailOrUsername);
        if (user is null)
        {
            return Result<LogincommandResponse>.Failure("Kullanıcı adı ya da şifre yanlış");
        }

        if (user.TFAIsCompleted is null || user.TFACode is null || user.TFAConfirmCode is null || user.TFAExpiresDate is null)
        {
            return Result<LogincommandResponse>.Failure("İki faktörlü kimlik doğrulama kodu eksik");
        }

        if (user.TFAIsCompleted.Value)
        {
            return Result<LogincommandResponse>.Failure("İki faktörlü kimlik doğrulama kodu geçersiz");
        }

        if (user.TFAExpiresDate.Value < DateTimeOffset.Now)
        {
            return Result<LogincommandResponse>.Failure("İki faktörlü kimlik doğrulama kodu süresi dolmuş");
        }

        if (user.TFACode?.Value != request.TFACode || user.TFAConfirmCode?.Value != request.TFAConfirmCode)
        {
            return Result<LogincommandResponse>.Failure("İki faktörlü kimlik doğrulama kodu yanlış");
        }

        user.SetTFACompleted();
        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var token = await jwtProvider.CreateTokenAsync(user, cancellationToken);
        var res = new LogincommandResponse
        {
            Token = token
        };
        return res;
    }
}