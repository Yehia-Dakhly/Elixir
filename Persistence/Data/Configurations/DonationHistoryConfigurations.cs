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
    internal class DonationHistoryConfigurations : IEntityTypeConfiguration<DonationHistory>
    {
        public void Configure(EntityTypeBuilder<DonationHistory> builder)
        {
            builder.ToTable(nameof(DonationHistory));
            builder.HasKey(d => d.Id);
            builder.HasOne(H => H.DonationCategory)
                .WithMany(D => D.DonationHistory)
                .HasForeignKey(H => H.DonationCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(H => H.Donor)
                .WithMany(D => D.DonationHistories)
                .HasForeignKey(H => H.DonorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(d => d.DonationDate)
                   .IsRequired();
        }
    }
}
