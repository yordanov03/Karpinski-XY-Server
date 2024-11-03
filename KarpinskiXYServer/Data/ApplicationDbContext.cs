using Karpinski_XY.Models;
using Karpinski_XY_Server.Data.Models.Base;
using Karpinski_XY_Server.Data.Models.Exhibition;
using Karpinski_XY_Server.Data.Models.Painting;
using Karpinski_XY_Server.Services.Contracts;
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

        public DbSet<Painting> Paintings { get; set; }
        public DbSet<PaintingImage> PaintingImages { get; set; }

        public DbSet<Exhibition> Exhibitions { get; set; }
        public DbSet<ExhibitionImage> ExhibitionImages { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Painting>()
            .HasMany(p => p.PaintingImages)
            .WithOne(pi => pi.Painting)
            .HasForeignKey(pi => pi.EntityId)
            .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Exhibition>()
                .HasMany(e => e.ExhibitionImages)
                .WithOne(ei => ei.Exhibition)
                .HasForeignKey(ei => ei.EntityId)
                .OnDelete(DeleteBehavior.SetNull);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            this.ApplyAuditInformation();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
        {
            this.ApplyAuditInformation();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void ApplyAuditInformation()
        {
            this.ChangeTracker
                .Entries()
                .ToList()
                .ForEach(entry =>
                {

                    if (entry.Entity is IEntity entity)
                    {
                        if (entry.State == EntityState.Added)
                        {
                            entity.CreatedOn = DateTime.UtcNow;
                        }
                        else if (entry.State == EntityState.Modified)
                        {
                            entity.ModifiedOn = DateTime.UtcNow;
                        }
                    }
                });
        }
    }
}