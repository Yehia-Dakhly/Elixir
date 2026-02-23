using System.ComponentModel.DataAnnotations.Schema;

namespace DomainLayer.Models
{
    public class NotificationChild : BaseEntity<long>
    {
        public bool IsRead { get; set; }
        public Guid UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public BloodDonationUser User { get; set; } = null!;
        [ForeignKey(nameof(NotificationBaseId))]
        public long NotificationBaseId { get; set; }
        public NotificationBase NotificationBase { get; set; } = null!;
    }
}
