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
    internal class GovernorateConfigurations : IEntityTypeConfiguration<Governorate>
    {
        public void Configure(EntityTypeBuilder<Governorate> builder)
        {
            builder.ToTable("Governorates");
            builder.Property(C => C.NameEn).IsRequired().HasMaxLength(50);
            builder.Property(C => C.NameAr).IsRequired().HasMaxLength(50);
        }
    }
}
