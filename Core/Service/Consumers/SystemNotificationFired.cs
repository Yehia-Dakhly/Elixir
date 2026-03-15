using DomainLayer.Models;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServiceAbstraction.Abstractions;
using Shared.Events;

namespace Service.Consumers
{
    public class SystemNotificationFired(
        UserManager<BloodDonationUser> _userManager,
        IFirebaseNotificationService _firebaseNotificationService
        ) : IConsumer<SystemNotificationEvent>
    {
        public async Task Consume(ConsumeContext<SystemNotificationEvent> context)
        {
            var Msg = context.Message;
            var UsersTokens = await _userManager.Users.Where(U =>
                (
                    (!Msg.GovernorateId.HasValue || U.City.GovernorateId == Msg.GovernorateId)
                    && (!Msg.BloodTypeId.HasValue || U.BloodTypeId == Msg.BloodTypeId)
                    && U.DeviceToken != null
                )
            ).Select(U => U.DeviceToken!).ToListAsync();

            await _firebaseNotificationService.SendToUsersAsync(UsersTokens, new Shared.DataTransferObjects.NotificationMessageDTo()
            {
                Title = Msg.Title,
                Body = Msg.Body,
            });
        }
    }
}
