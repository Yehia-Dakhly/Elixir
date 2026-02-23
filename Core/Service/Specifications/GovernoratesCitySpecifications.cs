using DomainLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications
{
    internal class GovernoratesCitySpecifications : BaseSpecifications<City, int>
    {
        public GovernoratesCitySpecifications(int id) : base(C => C.GovernorateId == id)
        {
        }
    }
}
