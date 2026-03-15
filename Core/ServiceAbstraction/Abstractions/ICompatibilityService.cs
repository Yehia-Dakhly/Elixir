using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction.Abstractions
{
    public interface ICompatibilityService
    {
        public Task<IEnumerable<int>> GetCompatibleBloodTypesIdsForSpecificBloodTypeWithCategoryAsync(int bloodTypeId, int donationCategoryId);
        public Task<IEnumerable<int>> GetDonorCompatibleBloodTypesIdsForAllCategoriesAsync(int bloodTypeId);
    }
}
