using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Exceptions
{
    public sealed class BadRequestException(List<string> errors) : Exception("خطأ في البيانات")
    {
        public List<string> Errors { get; private set; } = errors;
    }
}
