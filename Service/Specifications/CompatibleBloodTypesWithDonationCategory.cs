using DomainLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications
{
    internal class CompatibleBloodTypesWithDonationCategory : BaseSpecifications<CompatibilityMatrix, int>
    {
        public CompatibleBloodTypesWithDonationCategory(int RecipientBloodTypeId, int DonationCategoryId) : base(C => C.DonationCategoryId == DonationCategoryId && C.RecipientTypeId == RecipientBloodTypeId)
        {
        }
    }
}
