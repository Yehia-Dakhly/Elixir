using DomainLayer.Models;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Data
{
    public class BloodDonationDbContext(DbContextOptions<BloodDonationDbContext> options) : IdentityDbContext<BloodDonationUser, IdentityRole<Guid>, Guid>(options)
    {
        public DbSet<BloodDonationUser> Users { get; set; }
        public DbSet<BloodRequests> BloodRequests { get; set; }
        public DbSet<BloodTypes> BloodTypes { get; set; }
        public DbSet<CompatibilityMatrix> Compatibilities { get; set; }
        public DbSet<DonationCategories> DonationCategories { get; set; }
        public DbSet<DonationResponses> DonationResponses { get; set; }
        public DbSet<NotificationChild> Notifications { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Governorate> Governorates { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(AssemblyReference).Assembly);
            builder.Entity<BloodDonationUser>().ToTable("Users");
            builder.Entity<IdentityRole<Guid>>().ToTable("Roles");
            builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
            builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
        }
    }
}
