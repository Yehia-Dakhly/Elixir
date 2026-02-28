using DomainLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.RequestSpecifications
{
    internal class RequestAllCompletedCountSpecification : BaseSpecifications<BloodRequests, int>
    {
        public RequestAllCompletedCountSpecification() : base(R => R.Status == Status.Completed) 
        {
        }
    }
}
