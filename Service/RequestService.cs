using AutoMapper;
using DomainLayer.Contracts;
using DomainLayer.Exceptions;
using DomainLayer.Exceptions.NotFoundExceptions;
using DomainLayer.Models;
using DomainLayer.Optopns;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Service.Specifications;
using ServiceAbstraction;
using Shared;
using Shared.DataTransferObjects;
using Shared.Events;
using System.Security.Claims;

namespace Service
{
    public class RequestService(
        IUnitOfWork _unitOfWork,
        IMapper _mapper,
        UserManager<BloodDonationUser> _userManager,
        IPublishEndpoint _publishEndpoint,
        IHttpContextAccessor _httpContextAccessor,
        ICompatibilityService _compatibilityService,
        IOptionsSnapshot<BloodDonationSettings> _optionsSnapshot
        ) : IRequestService
    {
        public async Task<bool> CreateBloodRequestAsync(CreateBloodRequestDTo bloodRequestDTo, Guid RequesterId)
        {
            var Repo = _unitOfWork.GetRepository<BloodRequests, int>();

            //var OpenRequestsSpecification = new OpenRequestsForUserSpecification(RequesterId);
            //var HasActiveRequest = await Repo.GetAllAsync(OpenRequestsSpecification);
            //if (HasActiveRequest.Any())
            //    throw new BadRequestException(new List<string>() { "لديك طلب مفتوح بالفعل." });

            var AllTodaySpecification = new AllCreatedRequestsTodaySpecification(RequesterId);
            var TodayRequests = await Repo.GetAllAsync(AllTodaySpecification);
            if (TodayRequests.Count() >= _optionsSnapshot.Value.MaxBloodRequestsPerDay)
                throw new BadRequestException(new List<string>() { $"تجاوزت الحد الأقصى للطلبات اليومية ({_optionsSnapshot.Value.MaxBloodRequestsPerDay} طلبات). حاول غداً." });


            if (bloodRequestDTo.Deadline <= DateTime.UtcNow)
            {
                throw new BadRequestException(new List<string>() { "الموعد النهائي يجب أن يكون في المستقبل" });
            }
            var Request = _mapper.Map<BloodRequests>(bloodRequestDTo);

            Request.CreatedAt = DateTime.UtcNow;
            Request.Status = Status.Open;
            Request.RequesterId = RequesterId;

            await Repo.AddAsync(Request);
            var Result = await _unitOfWork.SaveChangesAsync();
            var Specification = new RequestWithRequesterBloodTypeDonationCategoryCity(Request.Id);
            var BloodRequest = await Repo.GetByIdAsync(Specification) ?? throw new BloodRequestNotFoundException(Request.Id);
            // RabbitMQ Consumer
            // Notify All Nearby Users With Compatible Donors!!!
            await _publishEndpoint.Publish(new BloodRequestCreatedEvent()
            {
                BloodCategoryName = BloodRequest.DonationCategory.NameAr,
                CityName = BloodRequest.City.NameAr,
                Latitude = BloodRequest.Latitude,
                Longitude = BloodRequest.Longitude,
                RequestId = BloodRequest.Id,
                DonationCategoryId = BloodRequest.DonationCategoryId,
                RequiredBloodTypeId = BloodRequest.RequiredBloodTypeId,
                IsDonorReported = false,
                RequesterId = RequesterId.ToString(),
                Description = BloodRequest.Description,
                PatientName = BloodRequest.PatientName,
                HospitalName = BloodRequest.HospitalName,
            });

            return Result > 0;
        }
        public async Task DeleteBloodRequestAsync(Guid RequesterId, int BloodRequestId)
        {
            var BRepo = _unitOfWork.GetRepository<BloodRequests, int>();
            var BloodRequest = await BRepo.GetByIdAsync(BloodRequestId);
            if (BloodRequest is null)
            {
                throw new BloodRequestNotFoundException(BloodRequestId);
            }
            if (BloodRequest.RequesterId != RequesterId)
            {
                throw new UnauthorizedException("غير مصرح بك لحذف هذا الطلب!");
            }
            if (BloodRequest.ResponsesCount > 0)
            {
                throw new ForbiddenException("لا يمكن حذف طلب عليه استجابات بالفعل. يمكنك إغلاقه بدلاً من حذفه.");
            }
            BRepo.Delete(BloodRequest);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task<PaginatedResult<BloodRequestDTo>> GetRequestsAsync(RequestQueryParams Params)
        {
            var Repo = _unitOfWork.GetRepository<BloodRequests, int>();
            IEnumerable<int>? CompatibleTypes = null;
            Guid? RequesterId = null;
            var UserIdString = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(UserIdString))
            {
                throw new UnauthorizedException("User Not Authorized");
            }
            if (Params.IsPersonalRequests == true)
            {
                RequesterId = Guid.Parse(UserIdString);
            }
            else if (Params.SuitableRequests == true)
            {
                var User = await _userManager.FindByIdAsync(UserIdString) ?? throw new UserNotFoundException(UserIdString);
                CompatibleTypes = await _compatibilityService.GetDonorCompatibleBloodTypesIdsForAllCategoriesAsync(User.BloodTypeId);
            }
            var Specification = new RequestGeneralSpecification(Params, CompatibleTypes, RequesterId);
            var Requests = await Repo.GetAllAsync(Specification);
            var CountSpecification = new RequestGeneralCountSpecification(Params);
            var Count = await Repo.CountAsync(Specification);
            var Data = _mapper.Map<IEnumerable<BloodRequestDTo>>(Requests);
            return new PaginatedResult<BloodRequestDTo>(Data.Count(), Params.PageNumber, Count, Data);
        }
        public async Task<BloodRequestDTo> GetRequestByIdAsync(int RequestId)
        {
            var Repo = _unitOfWork.GetRepository<BloodRequests, int>();
            var UserIdString = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(UserIdString))
            {
                throw new UnauthorizedException("User Not Authorized");
            }
            var Specification = new RequestByIdSpecification(RequestId);
            var Request = await Repo.GetByIdAsync(Specification) ?? throw new BloodRequestNotFoundException(RequestId);
            if (Request.Status != Status.Open)
            {
                if (Request.RequesterId.ToString() == UserIdString)
                {
                    return _mapper.Map<BloodRequestDTo>(Request);
                }
                else
                {
                    throw new ForbiddenException("غير مسموح لك بعرض هذا الطلب");
                }
            }
            else
            {
                return _mapper.Map<BloodRequestDTo>(Request);
            }
        }
        public async Task CloseBloodRequestAsync(Guid RequesterId, int RequestId)
        {
            var Repo = _unitOfWork.GetRepository<BloodRequests, int>();
            var Request = await Repo.GetByIdAsync(RequestId) ?? throw new BloodRequestNotFoundException(RequestId);
            if (Request.RequesterId != RequesterId)
                throw new UnauthorizedException("غير مسموح لك بقفل هذا الطلب");
            if (Request.Status == Status.Completed)
            {
                throw new ForbiddenException("لقد تم اكتمال هذا الطلب لا يمكنك غلقه");
            }
            if (Request.Status == Status.Closed)
            {
                throw new ForbiddenException("تم غلق هذا الطلب من قبل");
            }
            Request.Status = Status.Closed;
            await _unitOfWork.SaveChangesAsync();
        }
        DateTime GetEgyptTime()
        {
            try
            {
                var egyptZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time"); // Windows
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, egyptZone);
            }
            catch
            {
                var egyptZone = TimeZoneInfo.FindSystemTimeZoneById("Africa/Cairo"); // Linux/Mac
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, egyptZone);
            }
        }
    }
}