using DomainLayer.Contracts;
using DomainLayer.Models;
using DomainLayer.Optopns;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServiceAbstraction;
using Shared.DataTransferObjects;
using Shared.Events;

namespace Service.Consumers
{
    public class NotifyDonorsWithNewRequest(
        IServiceScopeFactory _scopeFactory,
        ILogger<NotifyDonorsWithNewRequest> _logger
        ) : IConsumer<BloodRequestCreatedEvent>
    {
        public async Task Consume(ConsumeContext<BloodRequestCreatedEvent> context)
        {
            #region Get Services
            using var Scope = _scopeFactory.CreateScope();
            var _compatibilityService = Scope.ServiceProvider.GetRequiredService<ICompatibilityService>();
            var _geoLocationService = Scope.ServiceProvider.GetRequiredService<IGeoLocationService>();
            var _unitOfWork = Scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var _bloodDonationSettings = Scope.ServiceProvider.GetRequiredService<IOptionsMonitor<BloodDonationSettings>>().CurrentValue;
            var _firebaseNotificationService = Scope.ServiceProvider.GetRequiredService<IFirebaseNotificationService>(); 
            #endregion

            var Msg = context.Message;
            _logger.LogInformation("Received BloodRequestCreatedEvent for RequestId: {RequestId}", Msg.RequestId);

            #region Get Compatible Donors IDs
            // Compatible Blood Types For Required Blood Type With Specific Donation Category, Because Some Blood Types Compatible With Other Blood Types Only In Specific Donation Category!
            var CompatibleBloodTypes = await _compatibilityService.GetCompatibleBloodTypesIdsForSpecificBloodTypeWithCategoryAsync(Msg.RequiredBloodTypeId, Msg.DonationCategoryId);
            var CompatibleUsersIds = new List<string>();
            foreach (var type in CompatibleBloodTypes)
            {
                var found = await _geoLocationService.GetNearbyDonorsIdsAsync(Msg.Longitude, Msg.Latitude, $"{type}");
                _logger.LogInformation("Found {CompatibleUsersCountInBloodType} compatible donors for BloodTypeId: {BloodTypeId} To RequestId: {RequestId}", found.Count(), type, Msg.RequestId);
                CompatibleUsersIds.AddRange(found);
            }
            CompatibleUsersIds = CompatibleUsersIds.ToList();
            #endregion

            if (CompatibleUsersIds.Any())
            {
                _logger.LogInformation("Found Total {TotalCompatibleUsers} compatible donors for RequestId: {RequestId}", CompatibleUsersIds.Count, Msg.RequestId);
                var NowDataTime = DateTime.UtcNow;
                #region Add Notifications!
                var BaseRepo = _unitOfWork.GetRepository<NotificationBase, long>();

                List<string> NewCompatibleUsersIds = new List<string>();

                var Base = new NotificationBase()
                {
                    SendAt = NowDataTime,
                    BloodRequestId = Msg.RequestId,
                    Title = NotificationProperties.GetRequestTitle(Msg.BloodCategoryName, Msg.HospitalName),
                    Body = NotificationProperties.GetRequestBody(Msg.PatientName, Msg.Description, Msg.CityName),
                    Data = new Dictionary<string, string>()
                        {
                            { "requestId", $"{Msg.RequestId}" }
                            // Add Action Here
                        },
                    NotificationType = NotificationType.NewBloodRequest,
                    NotificationChilderns = new List<NotificationChild>(),
                };
                
                foreach (var UserId in CompatibleUsersIds)
                {
                    if (UserId == Msg.RequesterId)
                    {
                        continue;
                    }
                    NewCompatibleUsersIds.Add(UserId);
                    Base.NotificationChilderns.Add(new NotificationChild()
                    {
                        NotificationBase = Base,
                        IsRead = false,
                        UserId = Guid.Parse(UserId)
                    });
                }
                await BaseRepo.AddAsync(Base);
                #endregion
                await _unitOfWork.SaveChangesAsync();

                #region Get Tokens From Redis
                var UsersTokens = await _geoLocationService.GetDonorsTokensAsync(NewCompatibleUsersIds); 
                #endregion

                if (UsersTokens is not null && UsersTokens.Any())
                {
                    var MaxDonorsToNotify = _bloodDonationSettings.MaxDonorsToNotify;
                    if (Msg.IsDonorReported.HasValue && Msg.IsDonorReported == true) // Some Donor Dosen't Attend To Donation And Reported That, So We Need To Notify More Donors!
                    {
                        _logger.LogInformation("Donor Reported For RequestId: {RequestId}, Notifying More Donors!", Msg.RequestId);
                        MaxDonorsToNotify = _bloodDonationSettings.MaxDonorsToSearchWhenAnoterDonorReported;
                    }
                    _logger.LogInformation("Notifying {DonorsCount} donors for RequestId: {RequestId}", UsersTokens.Count, Msg.RequestId);
                    var UsersTokenToChunks = UsersTokens.Chunk(MaxDonorsToNotify);

                    #region Send Notifications!
                    var UsersToNotify = UsersTokenToChunks.FirstOrDefault();
                    if (UsersToNotify is not null && UsersToNotify.Any())
                    {
                        await _firebaseNotificationService.SendToUsersAsync(UsersToNotify, new NotificationMessageDTo()
                        {
                            Title = NotificationProperties.GetRequestTitle(Msg.BloodCategoryName, Msg.HospitalName),
                            Body = NotificationProperties.GetRequestBody(Msg.PatientName, Msg.Description, Msg.CityName),
                            Data = new Dictionary<string, string>()
                            {
                                { "requestId", $"{Msg.RequestId}" } 
                                // Data
                            }
                        });
                    } 
                    #endregion
                }
            }
        }
    }
}
