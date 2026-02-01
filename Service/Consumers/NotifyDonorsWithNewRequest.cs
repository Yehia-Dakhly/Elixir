using DomainLayer.Contracts;
using DomainLayer.Models;
using DomainLayer.Optopns;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ServiceAbstraction;
using Shared.DataTransferObjects;
using Shared.Events;

namespace Service.Consumers
{
    public class NotifyDonorsWithNewRequest(
        IServiceScopeFactory _scopeFactory
        ) : IConsumer<BloodRequestCreatedEvent>
    {
        public async Task Consume(ConsumeContext<BloodRequestCreatedEvent> context)
        {
            using var Scope = _scopeFactory.CreateScope();
            var _compatibilityService = Scope.ServiceProvider.GetRequiredService<ICompatibilityService>();
            var _geoLocationService = Scope.ServiceProvider.GetRequiredService<IGeoLocationService>();
            var _unitOfWork = Scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var _bloodDonationSettings = Scope.ServiceProvider.GetRequiredService<IOptionsMonitor<BloodDonationSettings>>().CurrentValue;
            var _firebaseNotificationService = Scope.ServiceProvider.GetRequiredService<IFirebaseNotificationService>();
            #region Get Compatible Donors IDs
            var CompatibleBloodTypes = await _compatibilityService.GetCompatibleBloodTypesIdsForSpecificBloodTypeWithCategoryAsync(context.Message.RequiredBloodTypeId, context.Message.DonationCategoryId);
            var CompatibleUsersIds = new List<string>();
            foreach (var type in CompatibleBloodTypes)
            {
                CompatibleUsersIds.AddRange(await _geoLocationService.GetNearbyDonorsIdsAsync(context.Message.Longitude, context.Message.Latitude, $"{type}"));
            }
            CompatibleUsersIds = CompatibleUsersIds.Distinct().ToList();
            #endregion

            if (CompatibleUsersIds.Any())
            {
                var NowDataTime = DateTime.UtcNow;
                #region Add Notifications!
                var NotificationsBase = new List<NotificationChild>();
                var NotificationsChild = new List<NotificationBase>();
                var ChildRepo = _unitOfWork.GetRepository<NotificationBase, long>();
                List<string> NewCompatibleUsersIds = new List<string>();
                foreach (var UserId in CompatibleUsersIds)
                {
                    if (UserId == context.Message.RequesterId)
                    {
                        continue;
                    }
                    NewCompatibleUsersIds.Add(UserId);
                    var Child = new NotificationBase()
                    {
                        SendAt = NowDataTime,
                        BloodRequestId = context.Message.RequestId,
                        Title = NotificationProperties.GeneralTitle,
                        Body = NotificationProperties.GetRequestBody(context.Message.BloodCategoryName, context.Message.CityName),
                        Data = new Dictionary<string, string>()
                        {
                            // Add Action Here
                            { "requestId", $"{context.Message.RequestId}" }
                        },
                    };
                    NotificationsChild.Add(Child);

                    NotificationsBase.Add(new NotificationChild()
                    {
                        NotificationBase = Child,
                        IsRead = false,
                        UserId = Guid.Parse(UserId)
                    });
                }
                var Repo = _unitOfWork.GetRepository<NotificationChild, long>();
                await ChildRepo.AddRangeAsync(NotificationsChild);
                await Repo.AddRangeAsync(NotificationsBase);
                #endregion

                await _unitOfWork.SaveChangesAsync();

                #region Get Tokens From Redis And Send Notifications!
                // Get Tokens From Redis
                var UsersTokens = await _geoLocationService.GetDonorsTokensAsync(NewCompatibleUsersIds);
                if (UsersTokens is not null && UsersTokens.Any())
                {
                    var MaxDonorsToNotify = _bloodDonationSettings.MaxDonorsToNotify;
                    if (context.Message.IsDonorReported.HasValue && context.Message.IsDonorReported == true)
                    {
                        MaxDonorsToNotify = _bloodDonationSettings.MaxDonorsToSearchWhenAnoterDonorReported;
                    }
                    var UsersTokenToChunks = UsersTokens.Chunk(MaxDonorsToNotify);
                    // Send Notifications!
                    var UsersToNotify = UsersTokenToChunks.FirstOrDefault();
                    if (UsersToNotify is not null && UsersToNotify.Any())
                    {
                        await _firebaseNotificationService.SendToUsersAsync(UsersToNotify, new NotificationMessageDTo()
                        {
                            Title = NotificationProperties.GeneralTitle,
                            Body = NotificationProperties.GetRequestBody(context.Message.BloodCategoryName, context.Message.CityName),
                            Data = new Dictionary<string, string>()
                            {
                                { "requestId", $"{context.Message.RequestId}" } 
                                // Data
                            }
                        });
                    }
                }
                #endregion
            }
        }
    }
}
