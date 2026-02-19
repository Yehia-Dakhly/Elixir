
namespace DomainLayer.Models
{
    public class NotificationBase : BaseEntity<long>
    {
        public DateTime SendAt { get; set; }
        public int BloodRequestId { get; set; }
        public BloodRequests BloodRequest { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Body { get; set; } = null!;
        public Dictionary<string, string> Data { get; set; } = null!;
        public NotificationType NotificationType { get; set; }
        public ICollection<NotificationChild> NotificationChilderns { get; set; } = new HashSet<NotificationChild>();
    }
}
