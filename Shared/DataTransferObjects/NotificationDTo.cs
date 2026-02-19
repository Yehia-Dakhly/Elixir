
namespace Shared.DataTransferObjects
{
    public class NotificationDTo
    {
        public long Id { get; set; }
        public string Title { get; set; } = null!;
        public string Body { get; set; } = null!;
        public int BloodRequestId { get; set; }
        public DateTime CreatedAt { get; set; }
        public Dictionary<string, string> Data { get; set; } = null!;
        public bool IsRead { get; set; }
        public int NotificationType { get; set; }
    }
}
