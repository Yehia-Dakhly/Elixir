using DomainLayer.Contracts;
using DomainLayer.Exceptions;
using DomainLayer.Exceptions.NotFoundExceptions;
using DomainLayer.Models;
using MassTransit;
using MassTransit.Transports;
using Microsoft.Extensions.DependencyInjection;
using ServiceAbstraction;
using Shared.Events;

namespace Service.Consumers
{
    public class RequestResponsed(IServiceScopeFactory _serviceScopeFactory) : IConsumer<ResponseRequestedEvent>
    {
        public async Task Consume(ConsumeContext<ResponseRequestedEvent> context)
        {
            using var Scope = _serviceScopeFactory.CreateScope();
            var _fireBaseNotificationService = Scope.ServiceProvider.GetRequiredService<IFirebaseNotificationService>();
            var _unitOfWork = Scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var _publishEndpoint = Scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();
            var _requestsUpdate = Scope.ServiceProvider.GetRequiredService<IRequestsUpdate>();
            var Msg = context.Message;

            #region Add Response
            var DonationResponsesRepo = _unitOfWork.GetRepository<DonationResponses, long>();
            var Response = new DonationResponses()
            {
                BloodRequestId = Msg.BloodRequestId,
                DonorUserId = Msg.DonorId,
                ResponseAt = DateTime.UtcNow,
                ResponseStatus = ResponseStatus.Pending,
            };
            await DonationResponsesRepo.AddAsync(Response);
            #endregion

            #region Update Count Request Responses
            var BloodRequestsRepo = _unitOfWork.GetRepository<BloodRequests, int>();
            var BloodRequest = await BloodRequestsRepo.GetByIdAsync(Msg.BloodRequestId) ?? throw new BloodRequestNotFoundException(Msg.BloodRequestId);
            BloodRequest.ResponsesCount++;
            BloodRequestsRepo.Update(BloodRequest);
            #endregion
            await _requestsUpdate.UpdateRequestAsync(Msg.BloodRequestId, new { ResponsesCount = BloodRequest.ResponsesCount, CollectedCount = BloodRequest.CollectedBags });
            #region Add Donation Response To History
            var DonationHistoryRepo = _unitOfWork.GetRepository<DonationHistory, long>();
            await DonationHistoryRepo.AddAsync(new DonationHistory()
            {
                DonationCategoryId = BloodRequest.DonationCategoryId,
                DonorId = Msg.DonorId,
                DonationDate = DateTime.UtcNow
            });
            #endregion

            #region Add Notification
            var NotificationRepo = _unitOfWork.GetRepository<NotificationChild, long>();
            DateTime SendAt = DateTime.UtcNow;
            var NotificationChildRepo = _unitOfWork.GetRepository<NotificationBase, long>();
            var Child = new NotificationBase()
            {
                BloodRequestId = context.Message.RequestId,
                Title = NotificationProperties.DonorAcceptedTitle,
                Body = NotificationProperties.DonorAcceptedBody(context.Message.DonorName),
                SendAt = SendAt,
                Data = new Dictionary<string, string>()
                {
                    // Add Action Here
                    { "requestId", $"{Msg.RequestId}" }
                },
            };
            await NotificationChildRepo.AddAsync(Child);
            await NotificationRepo.AddAsync(new NotificationChild()
            {
                IsRead = false,
                UserId = Guid.Parse(context.Message.RequesterId),
                NotificationBase = Child
            }); 
            #endregion

            await _unitOfWork.SaveChangesAsync();

            #region Notify Requester
            await _publishEndpoint.Publish(new SendNotificationEvent()
            {
                Title = NotificationProperties.DonorAcceptedTitle,
                Body = NotificationProperties.DonorAcceptedBody(context.Message.DonorName),
                SendAt = SendAt,
                Data = new Dictionary<string, string>()
                {
                    // Add Action Here
                    { "requestId", $"{Msg.RequestId}" }
                },
            });
            #endregion
        }
    }
}
