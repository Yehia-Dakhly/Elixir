using AutoMapper;
using DomainLayer.Contracts;
using DomainLayer.Exceptions;
using DomainLayer.Exceptions.NotFoundExceptions;
using DomainLayer.Models;
using DomainLayer.Optopns;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Service.Specifications;
using Service.Specifications.RequestSpecifications;
using ServiceAbstraction.Abstractions;
using Shared;
using Shared.DataTransferObjects;
using Shared.Events;
using System.Security.Claims;

namespace Service.Services
{
    public class RequestService(
        IUnitOfWork _unitOfWork,
        IMapper _mapper,
        UserManager<BloodDonationUser> _userManager,
        IPublishEndpoint _publishEndpoint,
        ICompatibilityService _compatibilityService,
        IOptionsSnapshot<BloodDonationSettings> _optionsSnapshot,
        ILogger<RequestService> _logger
        ) : IRequestService
    {
        public async Task<bool> CreateBloodRequestAsync(CreateBloodRequestDTo bloodRequestDTo, Guid RequesterId)
        {
            var Repo = _unitOfWork.GetRepository<BloodRequests, int>();

            var OpenRequestsSpecification = new OpenRequestsForUserSpecification(RequesterId);
            var ActiveRequest = await Repo.GetByIdAsync(OpenRequestsSpecification);
            if (ActiveRequest != null)
            {
                _logger.LogWarning("User {UserId} attempted to create a new blood request but already has an open request with id {RequestId}", RequesterId, ActiveRequest.Id);
                throw new BadRequestException(new List<string>() { "لديك طلب مفتوح بالفعل." });
            }

            var AllTodaySpecification = new AllCreatedRequestsTodaySpecification(RequesterId);
            var TodayRequests = await Repo.GetAllAsync(AllTodaySpecification);
            if (TodayRequests.Count() >= _optionsSnapshot.Value.MaxBloodRequestsPerDay)
            {
                _logger.LogWarning("User {UserId} has reached the daily limit of blood requests with {RequestCount} requests today", RequesterId, TodayRequests.Count());
                throw new BadRequestException(new List<string>() { $"تجاوزت الحد الأقصى للطلبات اليومية ({_optionsSnapshot.Value.MaxBloodRequestsPerDay} طلبات). حاول غداً." });
            }

            if (bloodRequestDTo.Deadline <= DateTime.UtcNow)
            {
                _logger.LogWarning("User {UserId} attempted to create a blood request with a past deadline: {Deadline}", RequesterId, bloodRequestDTo.Deadline);
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
            _logger.LogInformation("User {UserId} created a new blood request with id {RequestId}", RequesterId, Request.Id);
            return Result > 0;
        }
        public async Task DeleteBloodRequestAsync(Guid RequesterId, int BloodRequestId)
        {
            var BRepo = _unitOfWork.GetRepository<BloodRequests, int>();
            var BloodRequest = await BRepo.GetByIdAsync(BloodRequestId);
            if (BloodRequest is null)
            {
                _logger.LogWarning("User {UserId} attempted to delete blood request with id {RequestId} but it was not found", RequesterId, BloodRequestId);
                throw new BloodRequestNotFoundException(BloodRequestId);
            }
            if (BloodRequest.RequesterId != RequesterId)
            {
                _logger.LogWarning("User {UserId} attempted to delete blood request with id {RequestId} but is not the owner", RequesterId, BloodRequestId);
                throw new UnauthorizedException("غير مصرح بك لحذف هذا الطلب!");
            }
            if (BloodRequest.ResponsesCount > 0)
            {
                _logger.LogWarning("User {UserId} attempted to delete blood request with id {RequestId} but it has responses", RequesterId, BloodRequestId);
                throw new ForbiddenException("لا يمكن حذف طلب عليه استجابات بالفعل. يمكنك إغلاقه بدلاً من حذفه.");
            }
            BRepo.Delete(BloodRequest);
            _logger.LogInformation("User {UserId} deleted blood request with id {RequestId}", RequesterId, BloodRequestId);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task<PaginatedResult<BloodRequestDTo>> GetRequestsAsync(RequestQueryParams Params, Guid RequesterId)
        {
            var Repo = _unitOfWork.GetRepository<BloodRequests, int>();
            IEnumerable<int>? CompatibleTypes = null;
            if (Params.SuitableRequests == true)
            {
                var User = await _userManager.FindByIdAsync(RequesterId.ToString()) ?? throw new UserNotFoundException(RequesterId);
                CompatibleTypes = await _compatibilityService.GetDonorCompatibleBloodTypesIdsForAllCategoriesAsync(User.BloodTypeId);
            }
            var Specification = new RequestGeneralSpecification(Params, CompatibleTypes, RequesterId);
            var Requests = await Repo.GetAllAsync(Specification);
            var CountSpecification = new RequestGeneralCountSpecification(Params);
            var Count = await Repo.CountAsync(CountSpecification);
            var Data = _mapper.Map<IEnumerable<BloodRequestDTo>>(Requests);
            return new PaginatedResult<BloodRequestDTo>(Data.Count(), Params.PageNumber, Count, Data);
        }
        public async Task<BloodRequestDTo> GetRequestByIdAsync(int RequestId, Guid UserId)
        {
            var Repo = _unitOfWork.GetRepository<BloodRequests, int>();
            var Specification = new RequestByIdSpecification(RequestId);
            var Request = await Repo.GetByIdAsync(Specification) ?? throw new BloodRequestNotFoundException(RequestId);
            if (Request.Status != Status.Open || Request.Deadline < DateTime.UtcNow)
            {
                if (Request.RequesterId == UserId)
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
            {
                _logger.LogWarning("User {UserId} attempted to close blood request with id {RequestId} but is not the owner", RequesterId, RequestId);
                throw new UnauthorizedException("غير مسموح لك بقفل هذا الطلب");
            }
            if (Request.Status == Status.Completed)
            {
                _logger.LogWarning("User {UserId} attempted to close blood request with id {RequestId} but it is already completed", RequesterId, RequestId);
                throw new ForbiddenException("لقد تم اكتمال هذا الطلب لا يمكنك غلقه");
            }
            if (Request.Status == Status.Closed)
            {
                _logger.LogWarning("User {UserId} attempted to close blood request with id {RequestId} but it is already closed", RequesterId, RequestId);
                throw new ForbiddenException("تم غلق هذا الطلب من قبل");
            }
            Request.Status = Status.Closed;
            _logger.LogInformation("User {UserId} closed blood request with id {RequestId}", RequesterId, RequestId);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task<PaginatedResult<PersonalRequestsDTo>> GetPersonalRequestsAsync(PersonalRequestsQueryParams queryParams, Guid DonorId)
        {
            var PersonalSpecification = new PersonalRequestsSpecification(DonorId, queryParams);
            var PersonalRequestsRepo = _unitOfWork.GetRepository<BloodRequests, int>();
            var Requests = await PersonalRequestsRepo.GetAllAsync(PersonalSpecification);
            var PersonalCountSpecification =  new PersonalRequestsCountSpecification(DonorId, queryParams);
            var PersonalRequestsCount = await PersonalRequestsRepo.CountAsync(PersonalCountSpecification);
            var MappedRequests = _mapper.Map<IEnumerable<PersonalRequestsDTo>>(Requests);
            return new PaginatedResult<PersonalRequestsDTo>(queryParams.Pagesize, queryParams.PageNumber, PersonalRequestsCount, MappedRequests);
        }
    }
}