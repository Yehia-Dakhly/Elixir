using DomainLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications
{
    internal class RequestResponsesByRequestId : BaseSpecifications<DonationResponses, long>
    {
        public RequestResponsesByRequestId(long id) : base(Q => Q.BloodRequestId == id)
        {
            AddInculde(Q => Q.DonorUser);
            AddInculde(Q => Q.DonorUser.BloodType);
            AddInculde(Q => Q.DonorUser.City);
            AddOrderBy(Q => Q.ResponseAt);
        }
    }
}
