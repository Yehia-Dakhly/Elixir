using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Exceptions.NotFoundExceptions
{
    public class BloodRequestNotFoundException(int id) : NotFoundException($"طلب التبرع رقم: {id} غير موجود!")
    {
    }
}
