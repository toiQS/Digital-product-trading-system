using DPTS.Infrastructures.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DPTS.Applications
{
    public static class Config
    {
        public static void Initalize(this IServiceCollection services, IConfiguration configuration)
        {
            services.InitializeConnect(configuration);
            services.InitializeService();
        }
        private static void InitializeConnect(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("PostgreConnectString")));
        }
        private static void InitializeService(this IServiceCollection services)
        {

            //services.AddScoped<IStatisticService, StatictisService>();
            //services.AddScoped<IAuthService, AuthService>();
            //services.AddScoped<IMessageService, MessageService>();
            //services.AddScoped<ICategoryService, CategoryService>();
            //services.AddScoped<IOrderService, OrderService>();
            //services.AddScoped<IProductService, ProductService>();
            //services.AddScoped<IUserService, UserService>();
            //services.AddScoped<IWalletService, WalletService>();
        }
    }
}
