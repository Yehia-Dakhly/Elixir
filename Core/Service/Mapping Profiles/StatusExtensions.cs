using DomainLayer.Models;

public static class StatusExtensions
{
    public static string ToArabic(this Status status)
    {
        return status switch
        {
            Status.Pending => "قيد الانتظار",
            Status.Open => "مفتوح",
            Status.Closed => "مغلق",
            Status.Completed => "مكتمل",
            _ => "غير معروف"
        };
    }
}