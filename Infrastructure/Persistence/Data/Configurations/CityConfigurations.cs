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
    internal class CityConfigurations : IEntityTypeConfiguration<City>
    {
        public void Configure(EntityTypeBuilder<City> builder)
        {
            builder.ToTable("Cities");
            builder.Property(C => C.NameEn).IsRequired().HasMaxLength(50);
            builder.Property(C => C.NameAr).IsRequired().HasMaxLength(50);

            builder.HasOne(C => C.Governorate)
               .WithMany(G => G.Cities)
               .HasForeignKey(C => C.GovernorateId)
               .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
