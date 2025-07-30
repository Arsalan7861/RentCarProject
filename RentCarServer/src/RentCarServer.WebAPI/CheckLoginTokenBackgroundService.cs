
using GenericRepository;
using Microsoft.EntityFrameworkCore;
using RentCarServer.Domain.LoginTokens;

namespace RentCarServer.WebAPI;

public sealed class CheckLoginTokenBackgroundService(
    IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var scope = serviceScopeFactory.CreateScope();
            var srv = scope.ServiceProvider;

            var loginTokenRepository = srv.GetRequiredService<ILoginTokenRepository>();
            var unitOfWork = srv.GetRequiredService<IUnitOfWork>();

            var now = DateTimeOffset.Now;
            var activeList = await loginTokenRepository
                .Where(x => x.IsActive.Value == true && x.ExpiresDate.Value < now)
                .ToListAsync(stoppingToken);

            foreach (var item in activeList)
            {
                item.SetIsActive(new(false));
            }
            
            if (activeList.Any())
            {
                loginTokenRepository.UpdateRange(activeList);
                await unitOfWork.SaveChangesAsync(stoppingToken);
            }

            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }
}
