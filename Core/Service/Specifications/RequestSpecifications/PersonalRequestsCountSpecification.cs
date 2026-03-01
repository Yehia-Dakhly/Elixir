using DomainLayer.Models;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.RequestSpecifications
{
    internal class PersonalRequestsCountSpecification : BaseSpecifications<BloodRequests, int>
    {
        public PersonalRequestsCountSpecification(Guid RequesterId, PersonalRequestsQueryParams queryParams) : base(R => R.RequesterId == RequesterId)
        {
        }
    }
}
