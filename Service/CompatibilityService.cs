using AutoMapper;
using DomainLayer.Contracts;
using DomainLayer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Service.Specifications;
using ServiceAbstraction;
using System.Text.Json;
using static MassTransit.ValidationResultExtensions;

namespace Service
{
    public class CompatibilityService(
        IUnitOfWork _unitOfWork,
        ICacheService _cacheService
        ) : ICompatibilityService
    {
        public async Task<IEnumerable<int>> GetCompatibleBloodTypesIdsForSpecificBloodTypeWithCategoryAsync(int bloodTypeId, int donationCategoryId)
        {
            var Key = $"recipient:{donationCategoryId}:{bloodTypeId}";
            var Cache = await _cacheService.GetAsync<List<int>>(Key);

            if (Cache is null || Cache.Count == 0)
            {
                var Repo = _unitOfWork.GetRepository<CompatibilityMatrix, int>();
                var Specification = new CompatibleBloodTypesWithDonationCategory(bloodTypeId, donationCategoryId);
                var AcceptedDonorsBloodTypesIds = (await Repo.GetAllAsync(Specification)).Select(A => A.DonorTypeId).ToList();
                await _cacheService.SetAsync(Key, AcceptedDonorsBloodTypesIds, TimeSpan.FromDays(7));
                return AcceptedDonorsBloodTypesIds;
            }
            return Cache;
        }

        public async Task<IEnumerable<int>> GetDonorCompatibleBloodTypesIdsForAllCategoriesAsync(int bloodTypeId)
        {
            var Key = $"donor:allcategories:{bloodTypeId}";
            var Cache = await _cacheService.GetAsync<List<int>>(Key);
            if (Cache is null || Cache.Count == 0)
            {
                var Repo = _unitOfWork.GetRepository<CompatibilityMatrix, int>();
                var Specification = new DonorCompatibleBloodTypesForAllCategoriesSpecification(bloodTypeId);
                var CompatibliesIds = (await Repo.GetAllAsync(Specification)).Select(A => A.DonorTypeId).ToList();
                await _cacheService.SetAsync(Key, CompatibliesIds, TimeSpan.FromDays(7));
                return CompatibliesIds;
            }
            return Cache;
        }
    }
}
