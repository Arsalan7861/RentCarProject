using RentCarServer.Application.Behaviors;
using System.Reflection;

namespace RentCarServer.Application.Services;
public sealed class PermissionService
{
    public List<string> GetAll()
    {
        var permissions = new HashSet<string>();
        permissions.Add("dashboard:view");

        var assembly = Assembly.GetExecutingAssembly();

        var types = assembly.GetTypes();

        foreach (var type in types)
        {
            var permissionAttr = type.GetCustomAttribute<PermissionAttribute>();
            if (permissionAttr != null && !string.IsNullOrEmpty(permissionAttr.Permission))
            {
                permissions.Add(permissionAttr.Permission);
            }
        }

        return permissions.ToList();
    }
}
