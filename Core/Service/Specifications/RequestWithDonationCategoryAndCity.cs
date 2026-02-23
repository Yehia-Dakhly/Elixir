using DomainLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications
{
    internal class RequestWithDonationCategoryAndCity : BaseSpecifications<BloodRequests, int>
    {
        public RequestWithDonationCategoryAndCity(int id) : base(R => R.Id == id)
        {
            AddInculde(R => R.City);
            AddInculde(R => R.DonationCategory);
        }
    }
}
