using DomainLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.DonationReponseSpecification
{
    internal class DonationResponseByRequestIdAndDonorId : BaseSpecifications<DonationResponses, long>
    {
        public DonationResponseByRequestIdAndDonorId(long RequestId, Guid DonorId) : base(R => R.BloodRequestId == RequestId && R.DonorUserId == DonorId)
        {

        }
    }
}
