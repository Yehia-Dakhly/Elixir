using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects
{
    public class CreateBloodRequestDTo
    {
        [Required(ErrorMessage = "اسم الحالة مطلوب")]
        [MaxLength(100, ErrorMessage = "الاسم طويل جداً")]
        [MinLength(3, ErrorMessage = "الاسم يجب أن يكون 3 حروف على الأقل")]
        public string PatientName { get; set; } = null!;
        [Required(ErrorMessage = "اسم المستشفى مطلوب")]
        [MaxLength(150)]
        public string HospitalName { get; set; } = null!;
        [Required(ErrorMessage = "الوصف مطلوب")]
        [MaxLength(500, ErrorMessage = "الوصف لا يجب أن يتعدى 500 حرف")]
        public string Description { get; set; } = null!;
        [Required]
        [Range(1, 6, ErrorMessage = "عدد المتبرعين يجب أن يكون بين 1 و 6")]
        public int BagsCount { get; set; }
        [Range(-90, 90, ErrorMessage = "خط العرض غير صحيح")]
        public double Latitude { get; set; }
        [Range(-180, 180, ErrorMessage = "خط الطول غير صحيح")]
        public double Longitude { get; set; }
        [Required]
        [Range(1, 4, ErrorMessage = "يجب اختيار فئة تبرع صحيحة")]
        public int DonationCategoryId { get; set; }
        [Required]

        [Range(1, 8, ErrorMessage = "يجب اختيار فصيلة دم")]
        public int RequiredBloodTypeId { get; set; }
        [Required]
        [Range(1, 396, ErrorMessage = "يجب اختيار مدينة")]
        public int CityId { get; set; }
        [Required(ErrorMessage = "يجب تحديد آخر موعد للاستلام")]
        public DateTime Deadline { get; set; }
        [Phone]
        [Required(ErrorMessage = "يجب تحديد رقم التواصل")]
        public string PhoneNumber { get; set; } = null!;
    }
}
