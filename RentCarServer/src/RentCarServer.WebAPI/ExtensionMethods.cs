using GenericRepository;
using RentCarServer.Application.Services;
using RentCarServer.Domain.Abstractions;
using RentCarServer.Domain.Branches;
using RentCarServer.Domain.Roles;
using RentCarServer.Domain.Shared;
using RentCarServer.Domain.Users;
using RentCarServer.Domain.Users.ValueObjects;

namespace RentCarServer.WebAPI;

public static class ExtensionMethods
{
    public static async Task CreateFirstUserAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope(); // Sadece bu scope içerisinde geçerli olacak şekilde bir scope oluşturuyoruz.
        var srv = scope.ServiceProvider;
        var userRepository = srv.GetRequiredService<IUserRepository>(); // IUserService servisini alıyoruz, bu servis kullanıcı işlemleri için kullanılıyor.
        var roleRepository = srv.GetRequiredService<IRoleRepository>(); // IRoleService servisini alıyoruz, bu servis rol işlemleri için kullanılıyor.
        var branchRepository = srv.GetRequiredService<IBranchRepository>(); // IBranchService servisini alıyoruz, bu servis şube işlemleri için kullanılıyor.
        var unitOfWork = srv.GetRequiredService<IUnitOfWork>(); // IUnitOfWork servisini alıyoruz, bu servis veritabanı işlemleri için kullanılıyor.

        Branch? branch = await branchRepository.FirstOrDefaultAsync(x => x.Name.Value == "Merkez Şube");
        Role? role = await roleRepository.FirstOrDefaultAsync(x => x.Name.Value == "sys_admin");

        if (branch == null) // Eğer veritabanında "Merkez Şube" adında bir şube yoksa
        {
            Name branchName = new("Merkez Şube");
            Address branchAddress = new(
                "İstanbul",
                "ATAŞEHİR",
                "İstanbul Merkez");
            Contact branchContact = new(
                "2121234567",
                "2121234568",
                "info@rentcar.com");
            branch = new Branch(branchName, branchAddress, branchContact, true);
            await branchRepository.AddAsync(branch); // Yeni şubeyi veritabanına ekliyoruz.
        }

        if (role == null) // Eğer veritabanında "sys_admin" adında bir rol yoksa
        {
            Name roleName = new("sys_admin");
            role = new Role(roleName, true);
            await roleRepository.AddAsync(role); // Yeni rolü veritabanına ekliyoruz.
        }

        if (!await userRepository.AnyAsync(x => x.UserName.Value == "admin")) // Eğer veritabanında hiç kullanıcı yoksa
        {
            FirstName firstName = new("Arsalan");
            LastName lastName = new("Khroush");
            Email email = new("arsalan@gmail.com");
            UserName username = new("admin");
            Password password = new("123");
            IdentityId branchId = branch.Id; // Şubenin ID'sini alıyoruz.
            IdentityId roleId = role.Id; // Rolün ID'sini alıyoruz.
            var user = new User(
                firstName,
                lastName,
                email,
                username,
                password,
                branchId,
                roleId,
                true
            );

            await userRepository.AddAsync(user); // Yeni kullanıcıyı veritabanına ekliyoruz.
            await unitOfWork.SaveChangesAsync(); // Değişiklikleri kaydediyoruz.
        }
    }

    public static async Task CleanRemovedPermissionsFromRoleAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope(); // Sadece bu scope içerisinde geçerli olacak şekilde bir scope oluşturuyoruz.
        var permissionCleanerService = scope.ServiceProvider.GetRequiredService<PermissionCleanerService>(); // PermissionCleanerService servisini alıyoruz, bu servis izinleri temizlemek için kullanılıyor.
        await permissionCleanerService.CleanRemovedPermissionsFromRoleAsync(); // İzinleri temizliyoruz.
    }
}
