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
    internal class DonationResponseConfigurations : IEntityTypeConfiguration<DonationResponses>
    {
        public void Configure(EntityTypeBuilder<DonationResponses> builder)
        {
            builder.ToTable("DonationResponses");

            builder.HasKey(R => R.Id);
            builder.HasOne(R => R.DonorUser)
                .WithMany(U => U.DonationResponses)
                .HasForeignKey(R => R.DonorUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(R => R.BloodRequest)
                .WithMany(Q => Q.DonationResponses)
                .HasForeignKey(R => R.BloodRequestId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
