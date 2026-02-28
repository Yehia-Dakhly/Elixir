using DomainLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.RequestSpecifications
{
    internal class RequestTotalCountSpecification : BaseSpecifications<BloodRequests, int>
    {
        public RequestTotalCountSpecification() : base(null)
        {
        }
    }
}
