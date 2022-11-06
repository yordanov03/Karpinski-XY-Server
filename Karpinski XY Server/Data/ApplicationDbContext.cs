using Karpinski_XY.Infrastructure.Services;
using Karpinski_XY.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Karpinski_XY.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        private readonly ICurrentUserService currentUserService;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
            ICurrentUserService currentUserService)
            : base(options)
        {
            this.currentUserService = currentUserService;
        }

        protected override void OnModelCreating(ModelBuilder builder) 
        {
            base.OnModelCreating(builder);
        }
    }
}