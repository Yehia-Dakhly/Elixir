using DomainLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.RequestSpecifications
{
    internal class RequestWithRequesterSpecification : BaseSpecifications<BloodRequests, int>
    {
        public RequestWithRequesterSpecification(int BloodRequestId) : base(R => R.Id == BloodRequestId)
        {
            AddInculde(R => R.Requester);
        }
    }
}
