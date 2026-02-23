using MassTransit;
using Shared.DataTransferObjects.Authentication.ResetForgetChangePasswordDTos;
using Shared.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Consumers
{
    public class SendEmail : IConsumer<SendEmailEvent>
    {
        public async Task Consume(ConsumeContext<SendEmailEvent> context)
        {
            await EmailSendHelper.SendEmailAsync(new Shared.Email()
            {
                Body = context.Message.Body,
                Subject = context.Message.Subject,
                To = context.Message.To
            });
        }
    }
}
