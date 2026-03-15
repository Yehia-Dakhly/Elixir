using AutoMapper;
using DomainLayer.Contracts;
using DomainLayer.Models;
using Service.Specifications;
using ServiceAbstraction.Abstractions;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class LookupService(IUnitOfWork _unitOfWork, IMapper _mapper) : ILookupService
    {
        public async Task<IEnumerable<BloodTypeDTo>> GetAllBloodTypesAsync()
        {
            var Repo = _unitOfWork.GetRepository<BloodTypes, int>();
            var BloodTypes = await Repo.GetAllAsync();
            var BloodTypesDTos = _mapper.Map<IEnumerable<BloodTypeDTo>>(BloodTypes);
            return BloodTypesDTos;
        }

        public async Task<IEnumerable<DonationCategoriesDTo>> GetAllCategoriesAsync()
        {
            var Repo = _unitOfWork.GetRepository<DonationCategories, int>();
            var DonationCategories = await Repo.GetAllAsync();
            var DonationCategoriesDTos = _mapper.Map<IEnumerable<DonationCategoriesDTo>>(DonationCategories);
            return DonationCategoriesDTos;
        }

        public async Task<IEnumerable<CityDTo>> GetAllCitiesDToAsync(int id)
        {
            var Repo = _unitOfWork.GetRepository<City, int>();
            var Specification = new GovernoratesCitySpecifications(id);
            var Cities = await Repo.GetAllAsync(Specification);
            var CitiesDTos = _mapper.Map<IEnumerable<CityDTo>>(Cities);
            return CitiesDTos;
        }

        public async Task<IEnumerable<GovernorateDTo>> GetAllGovernorateDToAsync()
        {
            var Repo = _unitOfWork.GetRepository<Governorate, int>();
            var Governorates = await Repo.GetAllAsync();
            var GovernorateDTos = _mapper.Map<IEnumerable<GovernorateDTo>>(Governorates);
            return GovernorateDTos;
        }
    }
}
