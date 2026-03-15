using DomainLayer.Contracts;
using DomainLayer.Exceptions;
using DomainLayer.Exceptions.NotFoundExceptions;
using DomainLayer.Models;
using MassTransit;
using MassTransit.Transports;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServiceAbstraction.Abstractions;
using Shared;
using Shared.Events;

namespace Service.Consumers
{
    public class RequestResponsed(
        IServiceScopeFactory _serviceScopeFactory,
        ILogger<RequestResponsed> _logger
        ) : IConsumer<ResponseRequestedEvent>
    {
        public async Task Consume(ConsumeContext<ResponseRequestedEvent> context)
        {
            #region Services
            using var Scope = _serviceScopeFactory.CreateScope();
            var _fireBaseNotificationService = Scope.ServiceProvider.GetRequiredService<IFirebaseNotificationService>();
            var _unitOfWork = Scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var _publishEndpoint = Scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();
            var _requestsUpdate = Scope.ServiceProvider.GetRequiredService<IRequestsUpdate>();
            #endregion
            var Msg = context.Message; 
            _logger.LogInformation("Received ResponseRequestedEvent for RequestId: {RequestId} from DonorId: {DonorId}", Msg.RequestId, Msg.DonorId);

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
            _logger.LogInformation("Updated BloodRequest with Id: {BloodRequestId} - New ResponsesCount: {ResponsesCount}", Msg.BloodRequestId, BloodRequest.ResponsesCount);
            #endregion

            await _requestsUpdate.UpdateRequestAsync(Msg.BloodRequestId, new RequestUpdateSignalRDTo { ResponsesCount = BloodRequest.ResponsesCount, CollectedCount = BloodRequest.CollectedBags });
            
            #region Add Donation Response To History
            var DonationHistoryRepo = _unitOfWork.GetRepository<DonationHistory, long>();
            await DonationHistoryRepo.AddAsync(new DonationHistory()
            {
                DonationCategoryId = BloodRequest.DonationCategoryId,
                DonorId = Msg.DonorId,
                DonationDate = DateTime.UtcNow
            });
            _logger.LogInformation("Added DonationHistory for DonorId: {DonorId} and BloodRequestId: {BloodRequestId}", Msg.DonorId, Msg.BloodRequestId);
            #endregion


            await _unitOfWork.SaveChangesAsync();

            #region Notify Requester
            await _publishEndpoint.Publish(new SendNotificationEvent()
            {
                Title = NotificationProperties.DonorAcceptedTitle,
                Body = NotificationProperties.DonorAcceptedBody(Msg.DonorName, Msg.PhoneNumber),
                SendAt = DateTime.UtcNow,
                Data = new Dictionary<string, string>()
                {
                    { "requestId", $"{Msg.RequestId}" }
                    // Add Action Here
                },
                DeviceToken = Msg.RequesterDeviceToken,
                BloodRequestId = Msg.BloodRequestId,
                NotificationType = (int)NotificationType.DonorAcceptedRequest,
                UserId = Guid.Parse(Msg.RequesterId)
            });
            #endregion
        }
    }
}
