using DomainLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications
{
    internal class RequestWithRequesterBloodTypeDonationCategoryCity : BaseSpecifications<BloodRequests, int>
    {
        public RequestWithRequesterBloodTypeDonationCategoryCity(int RequestId) : base(R => R.Id == RequestId)
        {
            AddInculde(R => R.City);
            AddInculde(R => R.RequiredBloodType);
            AddInculde(R => R.DonationCategory);
            AddInculde(R => R.Requester);
        }
        
    }
}
