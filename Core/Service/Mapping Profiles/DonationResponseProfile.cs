using AutoMapper;
using DomainLayer.Models;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Mapping_Profiles
{
    internal class DonationResponseProfile : Profile
    {
        public DonationResponseProfile()
        {
            CreateMap<DonationResponses, DonationResponseDTo>()
                .ForMember(R => R.FullName, O => O.MapFrom(R => R.DonorUser.FullName))
                .ForMember(R => R.Age, O => O.MapFrom(R => R.DonorUser.Age))
                .ForMember(R => R.BloodType, O => O.MapFrom(R => $"{R.DonorUser.BloodType.Symbol}{R.DonorUser.BloodType.RhFactor}"))
                .ForMember(R => R.CityName, O => O.MapFrom(R => R.DonorUser.City.NameAr))
                .ForMember(R => R.Gender, O => O.MapFrom(R => R.DonorUser.Gender.ToString()))
                .ForMember(R => R.ResponseStatus, O => O.MapFrom(R => R.ResponseStatus.ToString()))
                .ReverseMap();
        }
    }
}
