using GenericRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RentCarServer.Infrastructure.Context;
using RentCarServer.Infrastructure.Options;
using Scrutor;

namespace RentCarServer.Infrastructure;
public static class ServiceRegisterer
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection("Jwt")); // JWT ayarlarını alıyoruz.
        services.ConfigureOptions<JwtSetupOptions>(); // JWT ayarlarını uygulamak için JwtSetupOptions sınıfını kullanıyoruz.
        services.AddAuthentication().AddJwtBearer();
        services.AddAuthorization();

        // FluentEmail ayarlarını alıyoruz ve SMTP sunucusunu ekliyoruz.
        services.Configure<MailSettingsOptions>(configuration.GetSection("MailSettings"));

        using var scope = services.BuildServiceProvider().CreateScope(); // appjson dosyasındaki MailSettings bölümünü alabilmek için bir scope oluşturuyoruz.
        var mailSettings = scope.ServiceProvider.GetRequiredService<IOptions<MailSettingsOptions>>();
        if (string.IsNullOrEmpty(mailSettings.Value.UserId))
        {
            services.AddFluentEmail(mailSettings.Value.Email)
                .AddSmtpSender(
                mailSettings.Value.Smtp,
                mailSettings.Value.Port);
        }
        else
        {
            services.AddFluentEmail(mailSettings.Value.Email)
               .AddSmtpSender(
               mailSettings.Value.Smtp,
               mailSettings.Value.Port,
               mailSettings.Value.UserId,
               mailSettings.Value.Password);
        }

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            string con = configuration.GetConnectionString("SqlServer")!;
            options.UseSqlServer(con);
        });

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());

        // Scrutor kütüphanesini kullanarak servisleri tarayıp ekliyoruz, hepsini otomatik olarak ekleyecek class ile interface isimleri aynı olanları.
        services.Scan(action => action
        .FromAssemblies(typeof(ServiceRegisterer).Assembly)
        .AddClasses(publicOnly: false)
        .UsingRegistrationStrategy(RegistrationStrategy.Skip)
        .AsImplementedInterfaces()
        .WithScopedLifetime()
        );
        //services.AddScoped<IUserContext, UserContext>();
        return services;
    }
}
