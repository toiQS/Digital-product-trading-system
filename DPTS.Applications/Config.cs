using DPTS.Applications.Auth;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Data;
using DPTS.Infrastructures.Repository.Implements;
using DPTS.Infrastructures.Repository.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
            services.InitializeRepository();
            services.InitializeService();
            services.InitializeJwt(configuration);
        }

        private static void InitializeConnect(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("PostgreConnectString")));

        }

        private static void InitializeService(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(
                    //typeof(BuyerAssemblyMarker).Assembly,
                    //typeof(SellersAssemblyMarker).Assembly,
                    //typeof(NoDistinctionOfRoleAssemblyMarker).Assembly
                    typeof(AuthAssemblyMarker).Assembly
                );
            });
        }

        private static void InitializeRepository(this IServiceCollection services)
        {
           services.AddScoped<IAdjustmentRuleRepository, AdjustmentRuleRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IComplaintRepository, ComplaintRepository>();
            services.AddScoped<IEscrowRepository, EscrowRepository>();
            services.AddScoped<ILogRepository, LogRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IOrderItemRepository, OrderItemRepository>();
            services.AddScoped<IOrderPaymentRepository, OrderPaymentRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IPaymentMethodRepository,PaymentMethodRepository>();
            services.AddScoped<IProductAdjustmentRepository, ProductAdjustmentRepository>();
            services.AddScoped<IProductImageRepository, ProductImageRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductReviewRepository, ProductReviewRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IStoreRepository, StoreRepository>();
            services.AddScoped<IUserProfileRepository, UserProfileRepository>();
            services.AddScoped<IUserSecurityRepository, UserSecurityRepository>();
            services.AddScoped<IWalletRepository, WalletRepository>();
            services.AddScoped<IWalletTransactionRepository, WalletTransactionRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IEscrowProcessRepository, EscrowProcessRepository>();
            services.AddScoped<IAdjustmentHandle, AdjustmentHandle>();



        }
        private static void InitializeJwt(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {


                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration.GetSection("Jwt:Issuer").Value,
                    ValidAudience = configuration.GetSection("Jwt:Audience").Value,
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configuration.GetSection("Jwt:SecretKey").Value!))
                };
            });
        }
    }
}
