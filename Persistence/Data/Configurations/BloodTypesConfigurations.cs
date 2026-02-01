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
    internal class BloodTypesConfigurations : IEntityTypeConfiguration<BloodTypes>
    {
        public void Configure(EntityTypeBuilder<BloodTypes> builder)
        {
            builder.ToTable("BloodTypes");
            builder.Property(t => t.Symbol)
           .IsRequired()
           .HasMaxLength(2);
        }
    }
}
