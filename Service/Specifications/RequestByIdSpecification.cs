using DomainLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications
{
    internal class RequestByIdSpecification : BaseSpecifications<BloodRequests, int>
    {
        public RequestByIdSpecification(int id) : base(R => R.Id == id)
        {
            AddInculde(R => R.City);
            AddInculde(R => R.DonationCategory);
            AddInculde(R => R.Requester);
            AddInculde(R => R.RequiredBloodType);
        }
    }
}
