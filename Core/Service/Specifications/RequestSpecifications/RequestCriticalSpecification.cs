using DomainLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.RequestSpecifications
{
    internal class RequestCriticalSpecification : BaseSpecifications<BloodRequests, int>
    {
        public RequestCriticalSpecification()
            : base(R => R.Deadline > DateTime.Now && R.Deadline <= DateTime.Now.AddDays(1))
        {
        }
    }
}
