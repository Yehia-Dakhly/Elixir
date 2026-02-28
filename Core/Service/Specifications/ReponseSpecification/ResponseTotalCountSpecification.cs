using DomainLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.DonationReponseSpecification
{
    internal class ResponseTotalCountSpecification : BaseSpecifications<DonationResponses, long>
    {
        public ResponseTotalCountSpecification() : base(null)
        {
        }
    }
}
