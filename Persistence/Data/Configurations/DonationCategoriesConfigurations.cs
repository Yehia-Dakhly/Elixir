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
    internal class DonationCategoriesConfigurations : IEntityTypeConfiguration<DonationCategories>
    {
        public void Configure(EntityTypeBuilder<DonationCategories> builder)
        {
            builder.ToTable("DonationCategories");
            builder.Property(C => C.NameAr)
           .IsRequired()
           .HasMaxLength(55);
        }
    }
}
