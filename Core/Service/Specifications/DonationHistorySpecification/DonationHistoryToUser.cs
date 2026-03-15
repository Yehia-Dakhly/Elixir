using DomainLayer.Models;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.DonationHistorySpecification
{
    internal class DonationHistoryToUser : BaseSpecifications<DonationHistory, long>
    {
        public DonationHistoryToUser(DonationHistoryQueryParams Params, Guid UserId) : base(H =>  H.DonorId == UserId)
        {
            AddInculde(H => H.DonationCategory);
            switch (Params.DonationHistorySorting)
            {
                case DonationHistorySorting.DateAsc:
                    AddOrderBy(H => H.DonationDate);
                    break;
                case DonationHistorySorting.DateDesc:
                    AddOrderByDescending(H => H.DonationDate);
                    break;

                default:
                    AddOrderByDescending(H => H.DonationDate);
                    break;
            }
            ApplyPagination(Params.PageSize, Params.PageNumber);
        }
    }
}
