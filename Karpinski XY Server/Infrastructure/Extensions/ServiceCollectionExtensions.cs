using Karpinski_XY.Data;
using Karpinski_XY.Features.Identity;
using Karpinski_XY.Infrastructure.Filters;
using Karpinski_XY.Infrastructure.Services;
using Karpinski_XY.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Karpinski_XY.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static AppSettings GetApplicationSettings(this IServiceCollection services, IConfiguration configuration)
        {
            var applicationSettingsConfiguration = configuration.GetSection("ApplicationSettings");
            services.Configure<AppSettings>(applicationSettingsConfiguration);
            var appSettings = applicationSettingsConfiguration.Get<AppSettings>();
            return appSettings;
        }

        public static IServiceCollection AddDatabase(
            this IServiceCollection services, IConfiguration configuration)
            => services.AddDbContext<ApplicationDbContext>(options => options
            .UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        public static IServiceCollection AddIdentity(this IServiceCollection services)
        {
            services.AddDefaultIdentity<User>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 3;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>();

            return services;
        }

        public static IServiceCollection AddJWTAuthentication(this IServiceCollection services, AppSettings appSettings)
        {
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
             => services
                .AddTransient<IIdentityService, IdentityService>()
                .AddScoped<ICurrentUserService, CurrentUserService>();


        public static void AddApiControllers(this IServiceCollection services)
            => services.AddControllers(options => options.Filters.Add<ModelOrNotFoundActionFilter>());
    }
}
