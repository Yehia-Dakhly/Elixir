using AutoMapper;
using DomainLayer.Contracts;
using DomainLayer.Exceptions;
using DomainLayer.Exceptions.NotFoundExceptions;
using DomainLayer.Models;
using DomainLayer.Optopns;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Service.Consumers;
using Service.Helpers;
using Service.Specifications;
using ServiceAbstraction;
using Shared.DataTransferObjects;
using Shared.Events;
using System.Linq;

namespace Service
{
    public class ResponseService(
        IUnitOfWork _unitOfWork,
        IMapper _mapper,
        UserManager<BloodDonationUser> _userManager,
        //IConfiguration _configuration,
        IOptionsSnapshot<BloodDonationSettings> _optionsSnapshot,
        IPublishEndpoint _publishEndpoint,
        ICompatibilityService _compatibilityService
        ) : IResponseService
    {
        private readonly BloodDonationSettings _bloodDonationSettings = _optionsSnapshot.Value;
        public async Task<RespondBloodRequestDTo> RespondBloodRequestAsync(Guid DonorId, int BloodRequestId)
        {
            var Donor = await _userManager.FindByIdAsync(DonorId.ToString()) ?? throw new UserNotFoundException(DonorId);
            var DHRepo = _unitOfWork.GetRepository<DonationHistory, long>();
            var Spe = new DonationHistoryByUserId(DonorId);
            var LastDonationHistory = (await DHRepo.GetAllAsync(Spe)).FirstOrDefault();
            
            if (LastDonationHistory != null)
            {
                var MinDaysInterval = Donor.Gender == DomainLayer.Models.Gender.Male ? LastDonationHistory.DonationCategory.MaleMinDaysInterval : LastDonationHistory.DonationCategory.FemaleMinDaysInterval;
                if ((DateTime.UtcNow - LastDonationHistory.DonationDate).TotalDays > MinDaysInterval)
                {
                    return await TryDonate(DonorId, BloodRequestId, Donor);
                }
                else
                {
                    return new RespondBloodRequestDTo()
                    {
                        CanResponse = false,
                        LastDonationCatrgory = LastDonationHistory.DonationCategory.NameAr,
                        LastDonationDate = LastDonationHistory.DonationDate,
                        NextDonationDate = LastDonationHistory.DonationDate.AddDays(MinDaysInterval)
                    };
                }
            }
            return await TryDonate(DonorId, BloodRequestId, Donor);
        }
        private async Task<RespondBloodRequestDTo> TryDonate(Guid DonorId, int BloodRequestId, BloodDonationUser User)
        {

            var BRRepo = _unitOfWork.GetRepository<BloodRequests, int>();
            var BloodRequest = await BRRepo.GetByIdAsync(BloodRequestId) ?? throw new BloodRequestNotFoundException(BloodRequestId);
            // Check For Distance !!!
            var RequestLocationLong = BloodRequest.Longitude;
            var RequestLocationLatit = BloodRequest.Latitude;

            var UserLong = User.Longitude;
            var UserLatit = User.Latitude;
            var Distance = DistanceCalculator.CalculateDistance(RequestLocationLatit, RequestLocationLong, UserLatit, UserLong);
            if (Distance > _optionsSnapshot.Value.GovernorateRadius)
            {
                throw new BadRequestException(new List<string>() { "عفواً، لا يمكنك قبول الطلب لأنك بعيد جداً عن موقع الطلب" });
            }
            var CompatibleBloodTypes = (await _compatibilityService.GetCompatibleBloodTypesIdsForSpecificBloodTypeWithCategoryAsync(BloodRequest.RequiredBloodTypeId, BloodRequest.DonationCategoryId)).ToList();
            if (CompatibleBloodTypes.Any() && CompatibleBloodTypes.Contains(User.BloodTypeId))
            {
                if (BloodRequest.ResponsesCount < BloodRequest.BagsCount && BloodRequest.Status != Status.Completed)
                {
                    // Notify Requester!
                    DateTime ResponseTime = DateTime.UtcNow;
                    await _publishEndpoint.Publish(new ResponseRequestedEvent() // Publihser
                    {
                        RequesterId = $"{DonorId}",
                        DonorName = User.FullName,
                        PhoneNumber = User.PhoneNumber!, // 
                        RequestId = BloodRequestId,
                        ResponseAt = ResponseTime,
                        Status = Status.Pending.ToString(),
                        BloodRequestId = BloodRequestId,
                        DonorId = DonorId,
                        RequesterDeviceToken = BloodRequest.Requester.DeviceToken!,
                    });
                    return new RespondBloodRequestDTo() { CanResponse = true, PhoneNumber = BloodRequest.PhoneNumber };
                }
                throw new ForbiddenException("تم اكتمال الردود على هذا الطلب");
            }
            else
            {
                throw new ForbiddenException("فصيلة دمك غير مناسبة لهذا الطلب");
            }
        }
        public async Task<ConfirmRequestResponseDTo> ConfirmBloodRequestResponse(Guid RequesterId, Guid DonorId, int BloodRequestId, bool HasDonated)
        {
            var BRRepo = _unitOfWork.GetRepository<BloodRequests, int>();
            var Specification = new RequestWithDonationCategoryAndCity(BloodRequestId);
            var BloodRequest = await BRRepo.GetByIdAsync(Specification) ?? throw new BloodRequestNotFoundException(BloodRequestId); // Include Category
            if (BloodRequest.RequesterId != RequesterId)
            {
                throw new UnauthorizedException("لا يسمح لك بتأكيد هذا الطلب");
            }
            var Donor = await _userManager.FindByIdAsync(DonorId.ToString()) ?? throw new UserNotFoundException(DonorId);

            var ResponseRepo = _unitOfWork.GetRepository<DonationResponses, long>();
            var ResSpecification = new DonationResponseByBloodRequestAndDonorId(BloodRequestId, DonorId);

            var Response = (await ResponseRepo.GetAllAsync(ResSpecification)).FirstOrDefault() ?? throw new DonationResponseNotFound(BloodRequestId);
            if (Response.ResponseStatus == ResponseStatus.Arrived || Response.ResponseStatus == ResponseStatus.Rejected)
            {
                throw new ForbiddenException("تم التعامل مع هذا التبرع من قبل");
            }
            if (HasDonated == true)
            {
                BloodRequest.CollectedBags++;
                Response.ResponseStatus = ResponseStatus.Arrived;   
                await _unitOfWork.SaveChangesAsync();
                if (BloodRequest.CollectedBags >= BloodRequest.BagsCount)
                {
                    BloodRequest.Status = Status.Completed;
                    var Requester = await _userManager.FindByIdAsync(BloodRequest.RequesterId.ToString()) ?? throw new UserNotFoundException(RequesterId);
                    await _publishEndpoint.Publish(new SendNotificationEvent()
                        {
                            DeviceToken = Requester.DeviceToken!,
                            BloodRequestId = BloodRequestId,
                            Title = NotificationProperties.RequestCompletedTitle,
                            Body = NotificationProperties.RequestCompletedBody,
                            SendAt = DateTime.Now,
                            UserId = RequesterId,
                            Data = new Dictionary<string, string>()
                            {
                                // Date
                            },
                            NotificationType = (int)NotificationType.RequestCompleted,
                        });
                }

                await _publishEndpoint.Publish(new SendNotificationEvent()
                {
                    Title = NotificationProperties.DonationConfirmedTitle,
                    Body = NotificationProperties.DonationConfirmedBody,
                    BloodRequestId = BloodRequestId,
                    DeviceToken = Donor.DeviceToken!,
                    SendAt = DateTime.UtcNow,
                    UserId = Donor.Id,
                    NotificationType = (int)NotificationType.DonationConfirmed,
                    Data = new Dictionary<string, string>()
                    {
                        // Data
                    },
                });
                return new ConfirmRequestResponseDTo() { Success = true, Message = "تم تأكيد التبرع بنجاح" };
            }
            else
            {
                var PassedMinutes = (DateTime.UtcNow - Response.ResponseAt).TotalMinutes;
                if (PassedMinutes > _bloodDonationSettings.MaxTimeToCanRejectResponse) // 15 To appsettings
                {
                    Donor.MaxFailedDonationCount++;
                    await _userManager.UpdateAsync(Donor);
                    BloodRequest.ResponsesCount--;
                    Response.ResponseStatus = ResponseStatus.Rejected;
                    await _unitOfWork.SaveChangesAsync();
                    if (Donor.MaxFailedDonationCount > _bloodDonationSettings.MaxFailedDonationCount) // Add this 3 To appsettings
                    {
                        await _userManager.SetLockoutEndDateAsync(Donor, DateTimeOffset.MaxValue);
                        await _userManager.UpdateSecurityStampAsync(Donor);
                    }
                    // Search For Another Donor Here
                    await _publishEndpoint.Publish(new BloodRequestCreatedEvent()
                    {
                        IsDonorReported = true,
                        BloodCategoryName = BloodRequest.DonationCategory.NameAr,
                        CityName = BloodRequest.City.NameAr,
                        DonationCategoryId = BloodRequest.DonationCategoryId,
                        Latitude = BloodRequest.Latitude,
                        Longitude = BloodRequest.Longitude,
                        RequestId = BloodRequest.Id,
                        RequiredBloodTypeId = BloodRequest.RequiredBloodTypeId,
                        Description = BloodRequest.Description,
                        HospitalName = BloodRequest.HospitalName,
                        PatientName = BloodRequest.PatientName,
                        RequesterId = $"{BloodRequest.RequesterId}",
                    });
                    // Notify And Warn User!
                        await _publishEndpoint.Publish(new SendNotificationEvent()
                        {
                            BloodRequestId = BloodRequestId,
                            Title = NotificationProperties.DonationReportedTitle,
                            Body = NotificationProperties.DonationReportedBody(BloodRequest.PatientName),
                            DeviceToken = Donor.DeviceToken!,
                            SendAt = DateTime.UtcNow,
                            UserId = Donor.Id,
                            NotificationType = (int)NotificationType.DonationReported,
                            Data = new Dictionary<string, string>()
                            {
                                // Data
                            },
                        });
                    // Ask User If he donate or no, if he didn't Donate Then I'll - Remove - This Donation From Donation History!
                    return new ConfirmRequestResponseDTo { Success = true, Message = "تم تسجيل عدم الحضور وسيتم البحث عن متبرع آخر" }; ;
                }
                var RemainingMinutes = Math.Ceiling(_bloodDonationSettings.MaxTimeToCanRejectResponse - PassedMinutes);
                throw new ForbiddenException($"لا يمكنك الإبلاغ عن عدم الحضور الآن، يرجى الانتظار {RemainingMinutes} دقيقة.");
            }
        }
        public async Task<IEnumerable<DonationResponseDTo>> GetAllResponseByRequestId(Guid RequesterId, int BloodRequestId)
        {
            var RequestRepo = _unitOfWork.GetRepository<BloodRequests, int>();
            var Request = await RequestRepo.GetByIdAsync(BloodRequestId) ?? throw new BloodRequestNotFoundException(BloodRequestId);
            if (RequesterId != Request.RequesterId)
                throw new UnauthorizedException("غير مسموح لك بعرض ردود هذا الطلب");

            var ResRepo = _unitOfWork.GetRepository<DonationResponses, long>();
            var specification = new RequestResponsesByRequestId(BloodRequestId);
            var Responses = await ResRepo.GetAllAsync(specification);
            return _mapper.Map<IEnumerable<DonationResponseDTo>>(Responses);
        }
    }
}
