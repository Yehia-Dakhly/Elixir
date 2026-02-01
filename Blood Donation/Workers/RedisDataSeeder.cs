using DomainLayer.Models;
using DomainLayer.Optopns;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ServiceAbstraction;

namespace Blood_Donation.Workers
{
    public class RedisDataSeeder(ILogger<RedisDataSeeder> _logger, IServiceScopeFactory _scopeFactory) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    _logger.LogInformation("The process of transferring users data to Redis has begun...!");
                    var _userManager = scope.ServiceProvider.GetRequiredService<UserManager<BloodDonationUser>>();
                    var _geoService = scope.ServiceProvider.GetRequiredService<IGeoLocationService>();
                    var _bloodDonationSettings = scope.ServiceProvider.GetRequiredService<IOptionsMonitor<BloodDonationSettings>>().CurrentValue;

                    int DaysLimit = _bloodDonationSettings.DaysToUpdateDeviceToken;
                    var LastValidDate = DateTime.UtcNow.AddDays(-DaysLimit);
                    var AllUsers = await _userManager.Users
                        .Where(U => (U.LockoutEnd == null || U.LockoutEnd <= DateTime.UtcNow)
                        && U.Longitude != 0 && U.Latitude != 0 
                        && !string.IsNullOrEmpty(U.DeviceToken) 
                        && (U.LastTokenUpdate > LastValidDate))
                        .ToListAsync(stoppingToken); // stop Lock
                    int Count = 0;
                    foreach (var user in AllUsers)
                    {

                        await _geoService.UpdateDonorLocationAndDeviceTokenAsync($"{user.Id}", $"{user.DeviceToken}", user.Longitude, user.Latitude, $"{user.BloodTypeId}");

                        Count++;
                    }
                    _logger.LogInformation($"The operation was successful! {Count} Users has been added to Redis!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while transferring Users data to Redis!");
            }
        }
    }
}
