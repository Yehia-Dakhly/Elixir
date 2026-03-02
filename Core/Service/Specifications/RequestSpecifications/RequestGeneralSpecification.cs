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
    internal class RequestGeneralSpecification : BaseSpecifications<BloodRequests, int>
    {
        public RequestGeneralSpecification(RequestQueryParams Params, IEnumerable<int>? CompatibleTypesIdsForUser = null, Guid? UserId = null) : base(
            R => (!Params.GovernorateId.HasValue || R.City.GovernorateId == Params.GovernorateId)
            && (!Params.CityId.HasValue || R.CityId == Params.CityId)
            && (string.IsNullOrEmpty(Params.Search) || R.HospitalName.Contains(Params.Search))
            && (!Params.SuitableRequests || CompatibleTypesIdsForUser!.Contains(R.RequiredBloodTypeId))
            )
        {
            AddInculde(R => R.Requester);
            AddInculde(R => R.DonationCategory);
            AddInculde(R => R.City);
            AddInculde(R => R.RequiredBloodType);
            switch(Params.SortingOption)
            {
                case RequestSortingOptions.CollectedCountAsc:
                    AddOrderBy(R => R.CollectedBags);
                    break;

                case RequestSortingOptions.CollectedCountDesc:
                    AddOrderByDescending(R => R.CollectedBags);
                    break;

                case RequestSortingOptions.DeadlineAsc:
                    AddOrderBy(R => R.Deadline);
                    break;

                case RequestSortingOptions.DeadlineDesc:
                    AddOrderByDescending(R => R.Deadline);
                    break;

                case RequestSortingOptions.DataTimeAsc:
                    AddOrderBy(R => R.CreatedAt);
                    break;

                case RequestSortingOptions.DataTimeDesc:
                    AddOrderByDescending(R => R.CreatedAt);
                    break;

                default:
                    AddOrderBy(R => R.CreatedAt);
                    break;

            }
            ApplyPagination(Params.Pagesize, Params.PageNumber);
        }
    }
}
