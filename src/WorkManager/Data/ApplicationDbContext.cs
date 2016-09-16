using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WorkManager.Models;
using WorkManager.Models.Norms;

namespace WorkManager.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            builder.Entity<Timer>()
                .HasIndex(x => x.Started);
            builder.Entity<Timer>()
                .HasIndex(x => x.Stopped);

            builder.Entity<Project>()
                .HasIndex(x => x.OwnerId);
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<Timer> Timers { get; set; }
        public DbSet<Norm> Norms { get; set; }
    }
}
