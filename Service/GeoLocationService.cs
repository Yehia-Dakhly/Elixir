using DomainLayer.Contracts;
using DomainLayer.Exceptions;
using DomainLayer.Models;
using DomainLayer.Optopns;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Service.Specifications;
using ServiceAbstraction;
using Shared.DataTransferObjects;
using StackExchange.Redis;
using System.Drawing;
using static Google.Apis.Requests.BatchRequest;

namespace Service
{
    public class GeoLocationService(
        IConnectionMultiplexer _connectionMultiplexer,
        //IConfiguration _configuration,
        IOptionsSnapshot<BloodDonationSettings> _optionsSnapshot,
        IUnitOfWork _unitOfWork,
        UserManager<BloodDonationUser> _userManager
        ) : IGeoLocationService
    {
        private readonly IDatabase _database = _connectionMultiplexer.GetDatabase();
        private readonly BloodDonationSettings _bloodDonationSettings = _optionsSnapshot.Value;
        public async Task<IEnumerable<string>> GetNearbyDonorsIdsAsync(double Longitude, double Latitude, string BloodTypeId)
        {
            string key = $"geo:donors:{BloodTypeId}";

            var Donors = await GetDonorsInRadiusAsync(key, Longitude, Latitude, _bloodDonationSettings.CityRadius);
            if (Donors.Length < _bloodDonationSettings.MinDonorsCount)
            {
                Donors = await GetDonorsInRadiusAsync(key, Longitude, Latitude, _bloodDonationSettings.GovernorateRadius);
            }
            return Donors.Select(R => R.Member.ToString()).ToList();
        }
        private async Task<GeoRadiusResult[]> GetDonorsInRadiusAsync(string key, double longitude, double latitude, double Radius)
        {
            return await _database.GeoRadiusAsync(key, longitude, latitude, Radius, GeoUnit.Kilometers);
        }
        public async Task UpdateDonorLocationAndDeviceTokenAsync(string UserId, string DeviceToken, double Longitude, double Latitude, string BloodTypeId)
        {
            string key = $"geo:donors:{BloodTypeId}";
            await _database.GeoAddAsync(key, new GeoEntry(Longitude, Latitude, UserId));
            var User = await _userManager.FindByIdAsync(UserId);
            User!.LastTokenUpdate = DateTime.UtcNow;
            await _userManager.UpdateAsync(User);
            string infokey = $"donors:info:{UserId}";
            var Enteries = new HashEntry[]
            {
                new HashEntry("token", DeviceToken),
            };
            await _database.HashSetAsync(infokey, Enteries);
            await _database.KeyExpireAsync(infokey, TimeSpan.FromDays(_bloodDonationSettings.MinDaysToRemoveUserFromRAM)); // appsettings
        }
        public async Task<List<string>> GetDonorsTokensAsync(List<string> UsersIds)
        {
            var Batch = _database.CreateBatch();
            var Tasks = new List<Task<RedisValue>>();
            foreach (var UserId in UsersIds)
            {
                string key = $"donors:info:{UserId}";
                var Donor = await _userManager.FindByIdAsync(UserId);
                if (Donor == null) continue;
                var DHRepo = _unitOfWork.GetRepository<DonationHistory, long>();
                var Spe = new DonationHistoryByUserId(Guid.Parse(UserId));
                var LastDonationHistory = (await DHRepo.GetAllAsync(Spe)).FirstOrDefault();
                if (LastDonationHistory == null)
                {
                    Tasks.Add(Batch.HashGetAsync(key, "token"));
                    continue;
                }
                var MinDaysInterval = Donor.Gender == DomainLayer.Models.Gender.Male ? LastDonationHistory.DonationCategory.MaleMinDaysInterval : LastDonationHistory.DonationCategory.FemaleMinDaysInterval;
                if ((DateTime.UtcNow - LastDonationHistory.DonationDate).TotalDays > MinDaysInterval)
                {
                    Tasks.Add(Batch.HashGetAsync(key, "token"));
                }
                else
                {
                    continue;
                }
            }
            
            Batch.Execute();
            var Results = await Task.WhenAll(Tasks);
            return Results.Where(R => R.HasValue && !R.IsNullOrEmpty)
                .Select(R => R.ToString())
                .ToList();
        }
    }
}
