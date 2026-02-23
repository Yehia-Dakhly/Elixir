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
    internal class CompatibilityMatrixConfigurations : IEntityTypeConfiguration<CompatibilityMatrix>
    {
        public void Configure(EntityTypeBuilder<CompatibilityMatrix> builder)
        {
            builder.ToTable("BloodCompatibilities");

            builder.HasOne(Com => Com.RecipientType)
                .WithMany(T => T.RecipientCompatibilities)
                .HasForeignKey(c => c.RecipientTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(Com => Com.DonorType)
                .WithMany(T => T.DonorCompatibilities)
                .HasForeignKey(c => c.DonorTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(Com => Com.DonationCategory)
                .WithMany(C => C.Compatibilities)
                .HasForeignKey(c => c.DonationCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
