using DomainLayer.Exceptions;
using DomainLayer.Exceptions.NotFoundExceptions;
using DomainLayer.Models;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServiceAbstraction;
using Shared.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Service.Consumers
{
    public class UpdateLocationAndSendEmailForRegisteration(
        IServiceScopeFactory _serviceScopeFactory,
        ILogger<UpdateLocationAndSendEmailForRegisteration> _logger,
        IPublishEndpoint _publishEndpoint
        ) : IConsumer<UserRegisteredEvent>
    {
        public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
        {
            _logger.LogInformation("Received UserRegisteredEvent for UserId: {UserId}", context.Message.UserId);
            using var Scope = _serviceScopeFactory.CreateScope();
            var _userManager = Scope.ServiceProvider.GetRequiredService<UserManager<BloodDonationUser>>();
            var Msg = context.Message;
            var NewUser = await _userManager.FindByIdAsync(Msg.UserId) ?? throw new UserNotFoundException(Msg.UserId);
            var _geoLocationService = Scope.ServiceProvider.GetRequiredService<IGeoLocationService>();
            if (NewUser.DeviceToken is not null)
            {
                await _geoLocationService.UpdateDonorLocationAndDeviceTokenAsync($"{NewUser.Id}", NewUser.DeviceToken, NewUser.Longitude, NewUser.Latitude, $"{NewUser.BloodTypeId}");
            }
            await _publishEndpoint.Publish(new SendEmailEvent()
            {
                To = NewUser.Email!,
                Subject = "تأكيد بريدك الإلكتروني",
                Body = EmailSendHelper.GetConfirmRegisterEmailBody(Msg.ConfirmationLink!)
            });
        }
    }
}
