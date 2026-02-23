using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Exceptions.NotFoundExceptions
{
    public class NotificationNotFound(long Id) : NotFoundException($"لم يتم العثور على الإشعار رقم: {Id}!")
    {
    }
}
