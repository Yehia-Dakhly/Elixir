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
    internal class PersonalRequestsSpecification : BaseSpecifications<BloodRequests, int>
    {
        public PersonalRequestsSpecification(Guid RequesterId, PersonalRequestsQueryParams queryParams) : base(R => R.RequesterId == RequesterId)
        {
            AddOrderBy(R => R.CreatedAt);
            AddInculde(R => R.DonationResponses);
            AddInculde(R => R.Requester);
            AddInculde(R => R.DonationCategory);
            AddInculde(R => R.City);
            AddInculde(R => R.RequiredBloodType);
            ApplyPagination(queryParams.Pagesize, queryParams.PageNumber);
        }
    }
}
