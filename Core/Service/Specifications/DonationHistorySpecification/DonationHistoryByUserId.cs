using DomainLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.DonationHistorySpecification
{
    internal class DonationHistoryByUserId : BaseSpecifications<DonationHistory, long>
    {
        public DonationHistoryByUserId(Guid DonorId) : base(DH => DH.DonorId == DonorId)
        {
            AddInculde(DH => DH.DonationCategory);
            AddOrderByDescending(DH => DH.DonationDate);
        }
    }
}
