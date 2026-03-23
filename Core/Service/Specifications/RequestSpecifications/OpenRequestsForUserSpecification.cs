using DomainLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.RequestSpecifications
{
    internal class OpenRequestsForUserSpecification : BaseSpecifications<BloodRequests, int>
    {
        public OpenRequestsForUserSpecification(Guid UserId) : base(B => B.RequesterId == UserId && B.Status == Status.Open && B.Deadline >= DateTime.UtcNow)
        {
        }
    }
}
