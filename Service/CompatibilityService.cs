using AutoMapper;
using DomainLayer.Contracts;
using DomainLayer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Service.Specifications;
using ServiceAbstraction;
using System.Text.Json;

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
            var Cache = await _cacheService.GetAsync(Key);
            if (Cache is null)
            {
                var Repo = _unitOfWork.GetRepository<CompatibilityMatrix, int>();
                var Specification = new CompatibleBloodTypesWithDonationCategory(bloodTypeId, donationCategoryId);
                var AcceptedDonorsBloodTypesIds = (await Repo.GetAllAsync(Specification)).Select(A => A.DonorTypeId).ToList();
                var SerializedAcceptedDonorsBloodTypesIds = JsonSerializer.Serialize(AcceptedDonorsBloodTypesIds);
                await _cacheService.SetAsync(Key, SerializedAcceptedDonorsBloodTypesIds, TimeSpan.FromDays(360));
                return AcceptedDonorsBloodTypesIds;
            }
            var AccDonorsBloodTypesIds = JsonSerializer.Deserialize<IEnumerable<int>>(Cache);
            return AccDonorsBloodTypesIds!;
        }

        public async Task<IEnumerable<int>> GetDonorCompatibleBloodTypesIdsForAllCategoriesAsync(int bloodTypeId)
        {
            var Key = $"donor:allcategories:{bloodTypeId}";
            var Cache = await _cacheService.GetAsync(Key);
            if (Cache is null)
            {
                var Repo = _unitOfWork.GetRepository<CompatibilityMatrix, int>();
                var Specification = new DonorCompatibleBloodTypesForAllCategoriesSpecification(bloodTypeId);
                var CompatibliesIds = (await Repo.GetAllAsync(Specification)).Select(A => A.DonorTypeId).ToList();
                var SerializedCompatibliesIds = JsonSerializer.Serialize(CompatibliesIds);
                await _cacheService.SetAsync(Key, SerializedCompatibliesIds, TimeSpan.FromDays(360));
                return CompatibliesIds;
            }
            var DeSerializedCompatibliesIds = JsonSerializer.Deserialize<IEnumerable<int>>(Cache);
            return DeSerializedCompatibliesIds!;
        }
    }
}
