using DPTS.Domains;
using Microsoft.Extensions.DependencyInjection;

namespace DPTS.Infrastructures.Data;

public static class DataSeeder
{
    public static async Task SeedAllAsync(this IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

        if (context.Users.Any()) return;

        // Seed Roles
        var roles = new[]
        {
            new Role { RoleId = "admin", RoleName = "Admin", Description = "Admin" },
            new Role { RoleId = "buyer", RoleName = "Buyer", Description = "Buyer" },
            new Role { RoleId = "seller", RoleName = "Seller", Description = "Seller" }
        };
        await context.Roles.AddRangeAsync(roles);
    }
}
