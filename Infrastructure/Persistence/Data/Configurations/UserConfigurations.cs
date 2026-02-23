using DomainLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Data.Configurations
{
    internal class UserConfigurations : IEntityTypeConfiguration<BloodDonationUser>
    {
        public void Configure(EntityTypeBuilder<BloodDonationUser> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(x => x.Id);
            builder.HasOne(U => U.BloodType)
                .WithMany(T => T.Users)
                .HasForeignKey(U => U.BloodTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(U => U.City)
            .WithMany(C => C.Users)
            .HasForeignKey(U => U.CityId)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
