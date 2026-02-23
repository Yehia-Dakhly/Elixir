using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models
{
    public static class NotificationProperties
    {
        // =========================================================
        //       عند إنشاء طلب جديد (تُرسل للمتبرعين القريبين)
        // =========================================================
        public static string GeneralTitle { get => "❤️ قطرات من دمك = حياة لشخص آخر"; }

        public static string GetRequestTitle(string BloodDonationCategoryName, string HospitalName)
            //=> $"😡اصحى شوف اللي وراك يا زياد صلاح";
            => $"ساهم في إنقاذ حياة مريض بمستشفى {HospitalName} - تبرع بـ{BloodDonationCategoryName}";
        public static string GetRequestBody(string PatientName, string Description, string CityName)
            => $"{PatientName} - {Description} - مدينة {CityName}";
            //=> $"{PatientName} - {Description} - مدينة {CityName}";


        // =========================================================
        //       عند قبول المتبرع للطلب (تُرسل لطالب الدم)
        // =========================================================
        public static string DonorAcceptedTitle { get => "تم العثور على متبرع! 🩸"; }

        public static string DonorAcceptedBody(string DonorName, string DonorPhoneNumber)
            => $"المتبرع '{DonorName}' مستعد للحضور. تواصل معه فوراً على الرقم:\n{DonorPhoneNumber}";


        // =========================================================
        //   عند تأكيد الوصول والتبرع (تُرسل للمتبرع) ✅ (طلبك هنا)
        // =========================================================
        public static string DonationConfirmedTitle { get => "شكراً يا بطل.. لقد أنقذت حياة! ♥️"; }
        public static string DonationConfirmedBody
            => "تم تأكيد تبرعك بنجاح. كلمات الشكر لا توفيك حقك، جعلها الله في ميزان حسناتك وشفيعاً لك يوم القيامة! ♥️";
        // =========================================================
        //             اكتمال الطلب (تُرسل لطالب الدم)
        // =========================================================
        public static string RequestCompletedTitle { get => "تم اكتمال طلبك بنجاح 💯"; }

        public static string RequestCompletedBody { get => "الحمد لله على السلامة! تم توفير عدد المتبرعين المطلوب. نتمنى للمريض الشفاء العاجل! ♥️"; }


        // =========================================================
        // عند الإبلاغ عن عدم حضور المتبرع "No Show" (تُرسل للمتبرع المخالف)
        // =========================================================
        public static string DonationReportedTitle { get => "تنبيه بخصوص عدم الحضور ⚠️"; }
        public static string DonationReportedBody(string PatientName)
            => $"تم تسجيل عدم حضورك للتبرع لطلب {PatientName}. نرجو الالتزام مستقبلاً لعدم تعريض حياة المرضى للخطر وتجنب حظر الحساب.";
    }
}
