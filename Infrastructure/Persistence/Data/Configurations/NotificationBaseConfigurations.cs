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
    public class NotificationBaseConfigurations : IEntityTypeConfiguration<NotificationBase>
    {
        public void Configure(EntityTypeBuilder<NotificationBase> builder)
        {
            builder.ToTable("NotificationBase");
            builder.HasOne(N => N.BloodRequest)
                .WithMany(Q => Q.NotificationChild)
                .HasForeignKey(N => N.BloodRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(e => e.Data)
           .HasConversion(
               // 1. وأنت رايح للداتابيز: حول الـ Dictionary لنص JSON
               v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),

               // 2. وأنت راجع من الداتابيز: حول النص لـ Dictionary تاني
               v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions)null)
           );

            // معلومة إضافية (اختياري): لو عايز تقارن التغييرات صح عشان التعديل يشتغل
            // EF Core ساعات مبتعرفش إن الديكشنري اتغير محتواه، فبنضيف Comparer
            var valueComparer = new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<Dictionary<string, string>>(
                (c1, c2) => JsonSerializer.Serialize(c1, (JsonSerializerOptions)null) == JsonSerializer.Serialize(c2, (JsonSerializerOptions)null),
                c => c == null ? 0 : JsonSerializer.Serialize(c, (JsonSerializerOptions)null).GetHashCode(),
                c => JsonSerializer.Deserialize<Dictionary<string, string>>(JsonSerializer.Serialize(c, (JsonSerializerOptions)null), (JsonSerializerOptions)null));

            builder.Property(e => e.Data)
                   .Metadata
                   .SetValueComparer(valueComparer);
        }
    }
}
