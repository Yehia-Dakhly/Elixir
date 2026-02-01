using DomainLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications
{
    internal class CityWithGovernorateByCityId : BaseSpecifications<City, int>
    {
        public CityWithGovernorateByCityId(int CityId) : base(C => C.Id == CityId)
        {
            AddInculde(C => C.Governorate);
        }
    }
}
