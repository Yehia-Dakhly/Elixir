using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.DataTransferObjects.Authentication.ResetForgetChangePasswordDTos;
using Shared.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Consumers
{
    public class SendEmail(ILogger<SendEmail> _logger) : IConsumer<SendEmailEvent>
    {
        public async Task Consume(ConsumeContext<SendEmailEvent> context)
        {
            _logger.LogInformation("Received SendEmailEvent for {To}", context.Message.To);

            await EmailSendHelper.SendEmailAsync(new Shared.Email()
            {
                Body = context.Message.Body,
                Subject = context.Message.Subject,
                To = context.Message.To
            });
        }
    }
}
