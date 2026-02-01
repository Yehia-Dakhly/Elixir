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
        public static string NewRequestTitle(string bloodType)
            => $"نداء إنساني عاجل: مطلوب فصيلة {bloodType} 🚨";

        public static string NewRequestBody(string hospitalName)
            => $"هناك مريض في مستشفى {hospitalName} بحاجة ماسة لتبرعك. كن سبباً في نجاته!";
        public static string GeneralTitle { get => "❤️ قطرات من دمك = حياة لشخص آخر"; }
        
        public static string GetRequestBody(string BloodDonationCategoryName, string CityNameAr)
            => $"حالة بمدينة {CityNameAr} تحتاج للتبرع بـ{BloodDonationCategoryName}";


        // =========================================================
        //       عند قبول المتبرع للطلب (تُرسل لطالب الدم)
        // =========================================================
        public static string DonorAcceptedTitle { get => "قام متبرع بالرد على طلبك"; }

        public static string DonorAcceptedBody(string donorName)
            => $"قام المتبرع '{donorName}' بقبول طلبك وهو مستعد للحضور. يمكنك التواصل معه الآن.";


        // =========================================================
        //   عند تأكيد الوصول والتبرع (تُرسل للمتبرع) ✅ (طلبك هنا)
        // =========================================================
        public static string DonationConfirmedTitle { get => "شكراً لك يا بطل! 🦸‍♂️❤️"; }

        public static string DonationConfirmedBody { get => "قام طالب الدم بتأكيد استلام تبرعك. شكراً لأنك ساهمت في إنقاذ حياة إنسان، جعلها الله في ميزان حسناتك."; }

        // =========================================================
        //             اكتمال الطلب (تُرسل لطالب الدم)
        // =========================================================
        public static string RequestCompletedTitle { get => "تم اكتمال طلبك بنجاح ✅"; }

        public static string RequestCompletedBody { get => "الحمد لله على السلامة! تم توفير عدد الأكياس المطلوب. نتمنى للمريض الشفاء العاجل."; }


        // =========================================================
        // عند الإبلاغ عن عدم حضور المتبرع "No Show" (تُرسل للمتبرع المخالف)
        // =========================================================
        public static string DonationReportedTitle { get => "تنبيه بخصوص عدم الحضور ⚠️"; }

        public static string DonationReportedBody { get => "تم تسجيل عدم حضورك للتبرع بعد الموافقة. نرجو الالتزام مستقبلاً لعدم تعريض حياة المرضى للخطر وتجنب حظر الحساب."; }
        // =========================================================
        //          عند اعتذار المتبرع (تُرسل لطالب الدم)
        // =========================================================
        public static string DonorCancelledTitle { get => "تحديث بخصوص طلبك ⚠️"; }

        public static string DonorCancelledBody { get => "للأسف، اعتذر المتبرع عن الحضور. لا تقلق، يقوم النظام الآن بالبحث عن متبرع آخر فوراً..."; }
    }
}
