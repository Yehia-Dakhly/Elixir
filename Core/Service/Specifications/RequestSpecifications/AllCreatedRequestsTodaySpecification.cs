using DomainLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.RequestSpecifications
{
    internal class AllCreatedRequestsTodaySpecification : BaseSpecifications<BloodRequests, int>
    {
        public AllCreatedRequestsTodaySpecification(Guid UserId) : base(B => B.RequesterId == UserId && B.CreatedAt.Date == DateTime.UtcNow.Date)
        {
        }
    }
}
