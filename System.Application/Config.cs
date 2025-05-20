using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sys.Infrastructure.Data;

namespace Sys.Application
{
    public static class Config
    {
        public static void Initalize(this IServiceCollection services, IConfiguration configuration)
        {
            services.InitializeConnect(configuration);
        }
        private static void InitializeConnect(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("PostgreConnectString")));
        }
        
    }
}
