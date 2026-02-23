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
    internal class BloodRequestsConfigurations : IEntityTypeConfiguration<BloodRequests>
    {
        public void Configure(EntityTypeBuilder<BloodRequests> builder)
        {
            builder.ToTable("BloodRequests");
            builder.HasOne(Q => Q.Requester)
                .WithMany(U => U.BloodRequests)
                .HasForeignKey(Q => Q.RequesterId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(Q => Q.RequiredBloodType)
                .WithMany(T => T.BloodRequests)
                .HasForeignKey(Q => Q.RequiredBloodTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(Q => Q.DonationCategory)
                .WithMany(C => C.BloodRequests)
                .HasForeignKey(Q => Q.DonationCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(Q => Q.PatientName)
           .IsRequired()
           .HasMaxLength(100);

            builder.Property(Q => Q.HospitalName)
                   .IsRequired()
                   .HasMaxLength(150);

            builder.Property(Q => Q.Description)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.HasOne(Q => Q.City)
                .WithMany(C => C.BloodRequests)
                .HasForeignKey(Q => Q.CityId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
