using Microsoft.EntityFrameworkCore;
using RentCarServer.Application.Behaviors;
using RentCarServer.Domain.Branches;
using RentCarServer.Domain.Roles;
using RentCarServer.Domain.Users;
using TS.MediatR;
using TS.Result;

namespace RentCarServer.Application.Users;
[Permission("user:view")]
public sealed record UserGetQuery(
    Guid Id) : IRequest<Result<UserDto>>;

public sealed class UserGetQueryHandler(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IBranchRepository branchRepository) : IRequestHandler<UserGetQuery, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(UserGetQuery request, CancellationToken cancellationToken)
    {
        var res = await userRepository
            .GetAllWithAudit()
            .Mapto(roleRepository.GetAll(), branchRepository.GetAll())
            .Where(b => b.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (res is null)
        {
            return Result<UserDto>.Failure("Kullanıcı bulunamadı.");
        }

        return res;
    }
}
