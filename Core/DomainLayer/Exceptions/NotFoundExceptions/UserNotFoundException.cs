using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Exceptions.NotFoundExceptions
{
    public class UserNotFoundException : NotFoundException
    {
        public UserNotFoundException(Guid id)
            : base($"لم يتم العثور على المستخدم الذي ID: {id}!")
        {
        }
        public UserNotFoundException(string Email) 
            : base($"لم يتم العثور على المستخدم الذي يحمل البريد الإلكتروني: {Email}!")
        {
            
        }
    }
}
