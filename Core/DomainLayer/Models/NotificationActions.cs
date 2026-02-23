using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models
{
    public static class NotificationActions
    {
        // لما يكون فيه طلب تبرع جديد -> يفتح تفاصيل الطلب
        public const string OpenRequestDetails = "OPEN_REQUEST_DETAILS"; // Data: requestId

        public const string OpenRequestDonors = "OPEN_MY_REQUEST_DONORS"; // Data: requestId, donorId
        
        
        
        // لما حد يقبل تبرعك -> يفتح شاشة "تبرعاتي"
        public const string OpenMyDonations = "OPEN_MY_DONATIONS";

        // لما يوصلك رسالة شات -> يفتح الشات
        public const string OpenChat = "OPEN_CHAT";

        // إشعار عام (تحديث سيستم مثلاً) -> يفتح الصفحة الرئيسية
        public const string GeneralInfo = "GENERAL_INFO";
    }
}
