using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction.Abstractions
{
    public interface ILookupService
    {
        Task<IEnumerable<BloodTypeDTo>> GetAllBloodTypesAsync();
        Task<IEnumerable<DonationCategoriesDTo>> GetAllCategoriesAsync();
        Task<IEnumerable<GovernorateDTo>> GetAllGovernorateDToAsync();
        Task<IEnumerable<CityDTo>> GetAllCitiesDToAsync(int id);
    }
}
