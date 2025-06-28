using DPTS.Applications.Buyers;
using DPTS.Applications.NoDistinctionOfRoles;
using DPTS.Applications.Sellers;
using DPTS.Infrastructures.Data;
using DPTS.Infrastructures.Repository.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
                    typeof(BuyerAssemblyMarker).Assembly,   
                    typeof(SellersAssemblyMarker).Assembly,      
                    typeof(NoDistinctionOfRoleAssemblyMarker).Assembly                 
                );
            });
        }

        private static void InitializeRepository(this IServiceCollection services)
        {
            services.Scan(scan => scan
                    .FromAssemblyOf<ICategoryRepository>()
                    .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Repository")))
                    .AsMatchingInterface()
                    .WithScopedLifetime());

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
