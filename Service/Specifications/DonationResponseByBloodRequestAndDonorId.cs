using DomainLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications
{
    internal class DonationResponseByBloodRequestAndDonorId : BaseSpecifications<DonationResponses, long>
    {
        public DonationResponseByBloodRequestAndDonorId(int BloodRequestId, Guid DonorId) : base(R => R.BloodRequestId == BloodRequestId && R.DonorUserId == DonorId)
        {
        }
    }
}
