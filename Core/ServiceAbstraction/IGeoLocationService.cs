using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction
{
    public interface IGeoLocationService
    {
        Task UpdateDonorLocationAndDeviceTokenAsync(string UserId, string DeviceToken, double Longitude, double Latitude, string BloodTypeId);
        Task<IEnumerable<string>> GetNearbyDonorsIdsAsync(double Longitude, double Latitude, string BloodType);
        Task<List<string>> GetDonorsTokensAsync(List<string> UsersIds);
    }
}
