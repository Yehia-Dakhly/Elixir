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
    internal class RequestGeneralCountSpecification : BaseSpecifications<BloodRequests, int>
    {
        public RequestGeneralCountSpecification(RequestQueryParams Params, IEnumerable<int>? CompatibleTypesIdsForUser = null, Guid? UserId = null) : base(
            R => (!Params.GovernorateId.HasValue || R.City.GovernorateId == Params.GovernorateId)
            && (!Params.CityId.HasValue || R.CityId == Params.CityId)
            && (string.IsNullOrEmpty(Params.Search) || R.HospitalName.Contains(Params.Search))
            && (!Params.SuitableRequests || CompatibleTypesIdsForUser!.Contains(R.RequiredBloodTypeId))
            )
        {

        }
    }
}
