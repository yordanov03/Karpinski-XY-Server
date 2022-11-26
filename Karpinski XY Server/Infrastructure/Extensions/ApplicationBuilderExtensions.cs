using Karpinski_XY.Data;
using Microsoft.EntityFrameworkCore;

namespace Karpinski_XY.Infrastructure.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using var services = app.ApplicationServices.CreateScope();
            var dbContext = services.ServiceProvider.GetService<ApplicationDbContext>();
            dbContext.Database.Migrate();
        }
    }
}
