using DomainLayer.Contracts;
using DomainLayer.Models;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using ServiceAbstraction;
using Shared.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace Service.Consumers
{
    public class SendNotification(IServiceScopeFactory _scopeFactory) : IConsumer<SendNotificationEvent>
    {
        public async Task Consume(ConsumeContext<SendNotificationEvent> context)
        {
            var Scope = _scopeFactory.CreateScope();
            var _firebaseNotificationService = Scope.ServiceProvider.GetRequiredService<IFirebaseNotificationService>();
            var _unitOfWork = Scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var _NotificationBaseRepo = _unitOfWork.GetRepository<NotificationBase, long>();
            var _NotificationChildRepo = _unitOfWork.GetRepository<NotificationChild, long>();
            var Msg = context.Message;

            var Base = new NotificationBase()
            {
                SendAt = Msg.SendAt,
                BloodRequestId = Msg.BloodRequestId,
                Title = Msg.Title,
                Body = Msg.Body,
                Data = Msg.Data,
            };
            await _NotificationBaseRepo.AddAsync(Base);

            await _NotificationChildRepo.AddAsync(new NotificationChild()
            {
                NotificationBase = Base,
                IsRead = false,
                UserId = Msg.UserId
            });

            await _unitOfWork.SaveChangesAsync();

            if (Msg.DeviceToken is not null)
            {
                await _firebaseNotificationService.SendToUserAsync(Msg.DeviceToken, new Shared.DataTransferObjects.NotificationMessageDTo()
                {
                    Title = Msg.Title,
                    Body = Msg.Body,
                    IsRead = false,
                    SendAt = Msg.SendAt,
                    Data = Msg.Data,
                });
            }
        }
    }
}
