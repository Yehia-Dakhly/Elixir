using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Optopns
{
    public class BloodDonationSettings
    {
        public int MaxFailedDonationCount { get; set; }
        public int MaxTimeToCanRejectResponse { get; set; }
        public int MinDonorsCount { get; set; }
        public double CityRadius { get; set; }
        public double GovernorateRadius { get; set; }
        public int MinDaysToRemoveUserFromRAM { get; set; }
        public int MinutesToExpireOTPCode { get; set; }
        public int MaxDonorsToNotify { get; set; }
        public int MaxDonorsToSearchWhenAnoterDonorReported { get; set; }
        public int DaysToUpdateDeviceToken { get; set; }
        public int TokenLifespanInMinutes { get; set; }
        public int MaxBloodRequestsPerDay { get; set; }
    }
}