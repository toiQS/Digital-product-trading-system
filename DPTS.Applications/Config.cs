using DPTS.Applications.Implements;
using DPTS.Applications.Implements.statictis;
using DPTS.Applications.Interfaces;
using DPTS.Applications.Shareds.Implements;
using DPTS.Applications.Shareds.Interfaces;
using DPTS.Infrastructures.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Org.BouncyCastle.Asn1.X509.Qualified;

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
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IStatisticSellerService, StatisticSellerService>();
            services.AddScoped<IStatictisAdminService, StatictisAdminService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IUserService, UserService>();

        }
    }
}
