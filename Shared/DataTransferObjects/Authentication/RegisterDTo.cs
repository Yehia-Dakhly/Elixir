using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects.Authentication
{
    public class RegisterDTo
    {
        // Register
        // Take Email, FullName, BloodTypeId, Age, Gender, Latitude, Longitude, CityId
        // Return Token, Email, Display Name
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        public string FullName { get; set; } = null!;
        [Required]
        [Range(1, 8, ErrorMessage = "يرجى اختيار فئة دم صحيحة")]
        public int BloodTypeId { get; set; }
        [Required(ErrorMessage = "يجب تحديد تاريخ الميلاد")]
        public DateTime DateOfBirth { get; set; }
        [Required(ErrorMessage = "يجب اختيار النوع")]
        [Range(1, 2)]
        public int Gender { get; set; }
        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
        [Required]
        public double Latitude { get; set; }
        [Required]
        public double Longitude { get; set; }
        [Required(ErrorMessage = "يجب اختيار المدينة")]
        public int CityId { get; set; }
        [Required(ErrorMessage = "Device Token FCM is required")]
        public string DeviceToken { get; set; } = null!;
    }
}
