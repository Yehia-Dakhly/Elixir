using AutoMapper;
using DomainLayer.Contracts;
using DomainLayer.Models;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Service.Specifications;
using Service.Specifications.DonationReponseSpecification;
using Service.Specifications.RequestSpecifications;
using ServiceAbstraction.Abstractions;
using Shared;
using Shared.DataTransferObjects;
using Shared.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class DashboardService(
        IUnitOfWork _unitOfWork,
        IMapper _mapper,
        UserManager<BloodDonationUser> _userManager,
        IPublishEndpoint _publishEndpoint
        ) 
        : IDashboardService
    {
        public async Task<PaginatedResult<AdminNotificationDTo>> GetNotificationsForAdminAsync(AdminNotificationQueryParams queryParams)
        {
            var Repository = _unitOfWork.GetRepository<NotificationBase, long>();
            var Specification = new GeneralAdminNotificationSpecification(queryParams);
            var Notifications = await Repository.GetAllAsync(Specification);
            var CountSpecification = new GeneralAdminNotificationCountSpecification(queryParams);
            var TotalCount = await Repository.CountAsync(CountSpecification);
            var mappedNotifications = _mapper.Map<IEnumerable<AdminNotificationDTo>>(Notifications);
            return new PaginatedResult<AdminNotificationDTo>(queryParams.PageSize, queryParams.PageIndex, TotalCount, mappedNotifications);
        }
        public async Task<float> GetCompleteRequestsPercentageAsync()
        {
            var RequestRepo = _unitOfWork.GetRepository<BloodRequests, int>();
            var RequestTotalCountSpeci = new RequestTotalCountSpecification();
            var TotalRequestsCount = await RequestRepo.CountAsync(RequestTotalCountSpeci);

            if (TotalRequestsCount == 0)
            {
                return 0f;
            }

            var RequestTotalCompletedCountSpeci = new RequestAllCompletedCountSpecification();
            var CompletedRequestsCount = await RequestRepo.CountAsync(RequestTotalCompletedCountSpeci);


            float percentage = (float)CompletedRequestsCount / TotalRequestsCount * 100;

            return (float)Math.Round(percentage, 1);
        }
        public async Task<PaginatedResult<BloodRequestDTo>> GetCriticalRequestsAsync(RequestQueryParams Params)
        {
            var RequestRepo = _unitOfWork.GetRepository<BloodRequests, int>();
            var CriticalSpecification = new RequestCriticalSpecification();
            var Requests = await RequestRepo.GetAllAsync(CriticalSpecification);
            var MappedRequests = _mapper.Map<IEnumerable<BloodRequestDTo>>(Requests);
            var Count = await RequestRepo.CountAsync(CriticalSpecification);
            return new PaginatedResult<BloodRequestDTo>(MappedRequests.Count(), Params.PageNumber, Count, MappedRequests);
        }
        public async Task<DonorsBloodTypesAnalysis> GetDonorsDistributionPercentageAsync()
        {
            var BloodTypesCount = await _userManager.Users.GroupBy(U => U.BloodTypeId)
                .Select
                (
                    G => new
                    {
                        BloodTypeId = G.Key,
                        Count = G.Count(),
                    }
                ).ToListAsync();

            var TotalCount = BloodTypesCount.Sum(T => T.Count);

            if (TotalCount == 0)
            {
                return new DonorsBloodTypesAnalysis();
            }
            var BloodTypesPercentages = new BloodTypesPercentagesDTo();
            for (int i = 1; i <= 8; i++)
            {
                var Count = BloodTypesCount.FirstOrDefault(T => T.BloodTypeId == i)?.Count ?? 0;
                switch(i)
                {
                    case 1:
                        BloodTypesPercentages.APlus = (float)Math.Round((float)Count / TotalCount * 100, 1);
                        break;
                    case 2:
                        BloodTypesPercentages.AMinus = (float)Math.Round((float)Count / TotalCount * 100, 1);
                        break;
                    case 3:
                        BloodTypesPercentages.BPlus = (float)Math.Round((float)Count / TotalCount * 100, 1);
                        break;
                    case 4:
                        BloodTypesPercentages.BMinus = (float)Math.Round((float)Count / TotalCount * 100, 1);
                        break;
                    case 5:
                        BloodTypesPercentages.ABPlus = (float)Math.Round((float)Count / TotalCount * 100, 1);
                        break;
                    case 6:
                        BloodTypesPercentages.ABMinus = (float)Math.Round((float)Count / TotalCount * 100, 1);
                        break;
                    case 7:
                        BloodTypesPercentages.OPlus = (float)Math.Round((float)Count / TotalCount * 100, 1);
                        break;
                    case 8:
                        BloodTypesPercentages.OMinus = (float)Math.Round((float)Count / TotalCount * 100, 1);
                        break;
                    default:
                        break;
                }
            }
            return new DonorsBloodTypesAnalysis() {BloodTypesPercentagesDTo = BloodTypesPercentages, TotalDonorsCount = TotalCount };
        }
        public async Task<float> GetDonorsReadinessPercentageAsync()
        {
            var TotalDonors = await _userManager.Users.GroupBy(U => U.IsAvailable)
                .Select
                (
                    U => new 
                    {
                        IsAvailable = U.Key,
                        Count = U.Count(),
                    }
                ).ToListAsync();
            var TotalCount = TotalDonors.Sum(U => U.Count);
            if (TotalCount == 0)
            {
                return 0f;
            }
            var IAvailable = TotalDonors.FirstOrDefault(T => T.IsAvailable == true)?.Count ?? 0;
            float percentage = (float)Math.Round((float)IAvailable / TotalCount * 100, 1);

            return percentage;
        }
        public async Task<PaginatedResult<ElixirUserDTo>> GetElixirUsersAsync(UsersQueryParams queryParams)
        {
            var Users = _userManager.Users.Include(U => U.City).Where
                (
                    U =>
                    
                       (string.IsNullOrEmpty(queryParams.Search) || U.FullName.Contains(queryParams.Search))
                       && (!queryParams.IsAvailable.HasValue || U.IsAvailable == queryParams.IsAvailable)
                       && (!queryParams.CityId.HasValue || U.CityId == queryParams.CityId)
                       && (!queryParams.BloodType.HasValue || U.BloodTypeId == queryParams.BloodType)
                       && (!queryParams.GovernorateId.HasValue || U.City.GovernorateId == queryParams.GovernorateId)
                    
                ).Skip((queryParams.PageIndex - 1) * queryParams.Pagesize).Take(queryParams.Pagesize);
            var CountUsers = _userManager.Users.Include(U => U.City).Count
                (
                    U =>
                    
                       (string.IsNullOrEmpty(queryParams.Search) || U.FullName.Contains(queryParams.Search))
                       && (!queryParams.IsAvailable.HasValue || U.IsAvailable == queryParams.IsAvailable)
                       && (!queryParams.CityId.HasValue || U.CityId == queryParams.CityId)
                       && (!queryParams.BloodType.HasValue || U.BloodTypeId == queryParams.BloodType)
                       && (!queryParams.GovernorateId.HasValue || U.City.GovernorateId == queryParams.GovernorateId)
                    
                );
            /*
             * Skip = (PageIndex - 1) * PageSize;
             * Take = PageSize;
             */
            Users = queryParams.SortingOption switch
            {
                UsersSortingOptions.NameAsc => Users.OrderBy(U => U.FullName),
                UsersSortingOptions.NameDesc => Users.OrderByDescending(U => U.FullName),
                UsersSortingOptions.AgeAsc => Users.OrderBy(U => U.Age),
                UsersSortingOptions.AgeDesc => Users.OrderByDescending(U => U.Age),
                UsersSortingOptions.FaildAsc => Users.OrderBy(U => U.MaxFailedDonationCount),
                UsersSortingOptions.FaildDesc => Users.OrderByDescending(U => U.MaxFailedDonationCount),
                UsersSortingOptions.BloodTypeAsc => Users.OrderBy(U => U.BloodTypeId),
                UsersSortingOptions.BloodTypeDesc => Users.OrderByDescending(U => U.BloodTypeId),
                _ => Users.OrderBy(U => U.Id)
            };
            var ElixirUsers = await Users.ToListAsync();
            var MappedElixirUsers = _mapper.Map<IEnumerable<ElixirUserDTo>>(ElixirUsers);
            return new PaginatedResult<ElixirUserDTo>(Users.Count(), queryParams.Pagesize, CountUsers, MappedElixirUsers);
        }
        public async Task<float> GetResponsesCompletedPercentageAsync()
        {
            var ResponseRepo = _unitOfWork.GetRepository<DonationResponses, long>();
            var TotalCountSpecification = new ResponseTotalCountSpecification();
            var TotalCount = await ResponseRepo.CountAsync(TotalCountSpecification);
            if (TotalCount == 0)
            {
                return 0f;
            }

            var ConfirmedSpeification = new ResponseConfirmedSpeification();
            var ConfirmedCount = await ResponseRepo.CountAsync(ConfirmedSpeification);

            float Percentage = (float)ConfirmedCount / TotalCount * 100;

            return (float)Math.Round(Percentage, 1);
        }
        public async Task<float> GetResponsesFailedPercentageAsync()
        {
            var ResponseRepo = _unitOfWork.GetRepository<DonationResponses, long>();
            var TotalCountSpecification = new ResponseTotalCountSpecification();
            var TotalCount = await ResponseRepo.CountAsync(TotalCountSpecification);
            if (TotalCount == 0)
            {
                return 0f;
            }

            var FailedSpeification = new ResponseFailedSpeification();
            var FailedCount = await ResponseRepo.CountAsync(FailedSpeification);

            float Percentage = (float)FailedCount / TotalCount * 100;

            return (float)Math.Round(Percentage, 1);
        }
        public async Task SendSystemNotificationAsync(SystemNotificationQueryParams queryParams)
        {
            await _publishEndpoint.Publish(new SystemNotificationEvent()
            {
                Title = queryParams.Title,
                Body = queryParams.Body,
                BloodTypeId = queryParams.BloodTypeId,
                GovernorateId = queryParams.GovernorateId,
            });
        }
    }
}
