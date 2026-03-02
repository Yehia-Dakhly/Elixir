using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models
{
    public class BloodDonationUser : IdentityUser<Guid>
    {
        public string FullName { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public bool IsAvailable { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int BloodTypeId { get; set; }
        public BloodTypes BloodType { get; set; } = null!;
        public ICollection<BloodRequests> BloodRequests { get; set; } = new HashSet<BloodRequests>();
        public ICollection<DonationResponses> DonationResponses { get; set; } = new HashSet<DonationResponses>();
        public ICollection<NotificationChild> Notifications { get; set; } = new HashSet<NotificationChild>();
        public ICollection<DonationHistory> DonationHistories { get; set; } = new HashSet<DonationHistory>();
        public int CityId { get; set; }
        public City City { get; set; } = null!;
        public Gender Gender { get; set; }
        public int MaxFailedDonationCount { get; set; }
        public string? DeviceToken { get; set; }
        public DateTime LastTokenUpdate { get; set; }
        public string? ResetCode { get; set; }
        public DateTime? ExpireCodeTime { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new HashSet<RefreshToken>();
        [NotMapped]
        public int Age => DateTime.Today.Year - DateOfBirth.Year -
    (DateTime.Today.Month < DateOfBirth.Month || (DateTime.Today.Month == DateOfBirth.Month && DateTime.Today.Day < DateOfBirth.Day) ? 1 : 0);
    }
}
