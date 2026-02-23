using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Exceptions.NotFoundExceptions
{
    public class DonationResponseNotFound(long Id) : NotFoundException($"لم يتم العثور على استجابة التبرع بالدم في الطلب رقم: {Id}!")
    {
    }
}
