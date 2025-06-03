using DPTS.Domains;
using Microsoft.Extensions.DependencyInjection;

namespace DPTS.Infrastructures.Data
{
    public static class SeedingRole
    {
       public static async Task Initialize(this IServiceProvider serviceProvider)
        {
            try
            {
                var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
                if (!context.Roles.Any()) return;
                var arr = new[] { "Admin", "Buyer", "Seller" };
                foreach (var item in arr)
                {
                    await context.AddAsync(new Role()
                    {
                        RoleId = Guid.NewGuid().ToString(),
                        Description = item,
                        RoleName = item,
                    });
                }
                await context.SaveChangesAsync();
            }
            catch
            {
                throw new Exception();
            }
        }
    }
}
