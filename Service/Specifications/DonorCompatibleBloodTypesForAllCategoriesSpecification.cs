using DomainLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications
{
    internal class DonorCompatibleBloodTypesForAllCategoriesSpecification : BaseSpecifications<CompatibilityMatrix, int>
    {
        public DonorCompatibleBloodTypesForAllCategoriesSpecification(int DonorBloodType) : base(C => C.DonorTypeId == DonorBloodType)
        {
        }
    }
}
