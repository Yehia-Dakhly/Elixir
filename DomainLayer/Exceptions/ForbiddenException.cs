using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Exceptions
{
    public class ForbiddenException(string Msg = "ممنوع!") : Exception(Msg)
    {
    }
}
