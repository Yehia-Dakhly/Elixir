using DomainLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Persistence.Data.Configurations
{
    internal class NotificationLogsConfigurations : IEntityTypeConfiguration<NotificationChild>
    {
        public void Configure(EntityTypeBuilder<NotificationChild> builder)
        {
            builder.ToTable("NotificationChild");

            builder.HasOne(N => N.User)
                .WithMany(Q => Q.Notifications)
                .HasForeignKey(N => N.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(N => N.NotificationBase)
                .WithMany(N => N.NotificationChilderns)
                .HasForeignKey(N => N.NotificationBaseId)
                .OnDelete(DeleteBehavior.Restrict);
                
        }
    }
}
