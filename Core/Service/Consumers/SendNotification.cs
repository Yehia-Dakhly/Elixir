using DomainLayer.Contracts;
using DomainLayer.Models;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
    public class SendNotification(
        IServiceScopeFactory _scopeFactory,
        ILogger<SendNotification> _logger
        ) : IConsumer<SendNotificationEvent>
    {
        public async Task Consume(ConsumeContext<SendNotificationEvent> context)
        {
            #region Services
            using var Scope = _scopeFactory.CreateScope();
            var _firebaseNotificationService = Scope.ServiceProvider.GetRequiredService<IFirebaseNotificationService>();
            var _unitOfWork = Scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var _NotificationBaseRepo = _unitOfWork.GetRepository<NotificationBase, long>(); 
            #endregion

            var Msg = context.Message;
            _logger.LogInformation($"Received SendNotificationEvent for UserId: {Msg.UserId} with Title: {Msg.Title}");

            #region Add Notifications!
            var Base = new NotificationBase()
            {
                SendAt = Msg.SendAt,
                BloodRequestId = Msg.BloodRequestId,
                Title = Msg.Title,
                Body = Msg.Body,
                Data = Msg.Data,
                NotificationType = (NotificationType)Msg.NotificationType,
                NotificationChilderns = new List<NotificationChild>(),
            };
            Base.NotificationChilderns.Add(new NotificationChild()
            {
                NotificationBase = Base,
                IsRead = false,
                UserId = Msg.UserId
            });
            await _NotificationBaseRepo.AddAsync(Base); 
            #endregion
            
            await _unitOfWork.SaveChangesAsync();

            if (Msg.DeviceToken is not null)
            {
                _logger.LogInformation($"Sending notification to UserId: {Msg.UserId}.");
                await _firebaseNotificationService.SendToUserAsync(Msg.DeviceToken, new Shared.DataTransferObjects.NotificationMessageDTo()
                {
                    Title = Msg.Title,
                    Body = Msg.Body,
                    //IsRead = false,
                    //SendAt = Msg.SendAt,
                    Data = Msg.Data,
                });
            }
        }
    }
}
