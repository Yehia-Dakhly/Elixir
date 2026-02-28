using AutoMapper;
using DomainLayer.Contracts;
using DomainLayer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Service.Specifications;
using Service.Specifications.DonationReponseSpecification;
using Service.Specifications.RequestSpecifications;
using ServiceAbstraction;
using Shared;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class DashboardService(IUnitOfWork _unitOfWork, IMapper _mapper, UserManager<BloodDonationUser> _userManager) : IDashboardService
    {
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


            float percentage = ((float)CompletedRequestsCount / TotalRequestsCount) * 100;

            return (float)Math.Round(percentage, 1);
        }

        public async Task<PaginatedResult<BloodRequestDTo>> GetCriticalRequestsAsync(RequestQueryParams Params)
        {
            var RequestRepo = _unitOfWork.GetRepository<BloodRequests, int>();
            var CriticalSpecification = new RequestCriticalSpecification();
            var Requests = await RequestRepo.GetAllAsync(CriticalSpecification);
            var MappedRequests = _mapper.Map<IEnumerable<BloodRequestDTo>>(Requests);
            var Count = await RequestRepo.CountAsync(CriticalSpecification);
            return new PaginatedResult<BloodRequestDTo>(Params.Pagesize, Params.PageNumber, Count, MappedRequests);
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
                        BloodTypesPercentages.APlus = (float)Math.Round(((float)Count / TotalCount) * 100, 1);
                        break;
                    case 2:
                        BloodTypesPercentages.AMinus = (float)Math.Round(((float)Count / TotalCount) * 100, 1);
                        break;
                    case 3:
                        BloodTypesPercentages.BPlus = (float)Math.Round(((float)Count / TotalCount) * 100, 1);
                        break;
                    case 4:
                        BloodTypesPercentages.BMinus = (float)Math.Round(((float)Count / TotalCount) * 100, 1);
                        break;
                    case 5:
                        BloodTypesPercentages.ABPlus = (float)Math.Round(((float)Count / TotalCount) * 100, 1);
                        break;
                    case 6:
                        BloodTypesPercentages.ABMinus = (float)Math.Round(((float)Count / TotalCount) * 100, 1);
                        break;
                    case 7:
                        BloodTypesPercentages.OPlus = (float)Math.Round(((float)Count / TotalCount) * 100, 1);
                        break;
                    case 8:
                        BloodTypesPercentages.OMinus = (float)Math.Round(((float)Count / TotalCount) * 100, 1);
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
            float percentage = (float)Math.Round(((float)IAvailable / TotalCount) * 100, 1);

            return percentage;
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

            float Percentage = ((float)ConfirmedCount / TotalCount) * 100;

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

            float Percentage = ((float)FailedCount / TotalCount) * 100;

            return (float)Math.Round(Percentage, 1);
        }
    }
}
