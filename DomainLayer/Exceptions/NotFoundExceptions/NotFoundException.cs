using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Exceptions.NotFoundExceptions
{
    public abstract class NotFoundException(string Msg) : Exception(Msg)
    {
    }
}
