using FluentValidation;
using Karpinski_XY.Data;
using Karpinski_XY.Models;
using Karpinski_XY_Server.Data.Models.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NetCore.AutoRegisterDi;
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
            .UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

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

        public static IServiceCollection AddSmtpSettings(this IServiceCollection services, IConfiguration configuration)
        => services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"))
                    .Configure<ImageFiles>(configuration.GetSection("ImageFiles"))
                    .Configure<MailchimpSettings>(configuration.GetSection("Mailchimp"));

        public static void AddApplicationServices(this IServiceCollection services)
             => services.RegisterAssemblyPublicNonGenericClasses()
                    .Where(c => c.Name.EndsWith("Service"))
                    .AsPublicImplementedInterfaces(ServiceLifetime.Scoped);
        public static IServiceCollection AddApplicationValidators(this IServiceCollection services)
        {
            services.Scan(scan => scan
            .FromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
            .AddClasses(classes => classes.AssignableTo(typeof(IValidator<>)).Where(x => x.Name.EndsWith("Validator")))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

            return services;

        }


        //public static void AddApiControllers(this IServiceCollection services)
        //=> services.AddControllers(options => options.Filters.Add<ModelOrNotFoundActionFilter>());

        public static IServiceCollection AddAutoMapperConfiguration(this IServiceCollection services)
            => services.AddAutoMapper(typeof(Program));

        public static string SchemaSuffixStrategy(Type currentClass)
        {
            string suffix = "DTO";
            string returnedValue = currentClass.Name;
            if (returnedValue.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
                returnedValue = returnedValue.Replace(suffix, string.Empty, StringComparison.OrdinalIgnoreCase);
            return returnedValue;
        }
    }
}
