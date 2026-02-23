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
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<BloodDonationUser, AccountDTo>()
                .ForMember(U => U.CityName, O => O.MapFrom(U => U.City.NameAr))
                .ForMember(U => U.Gender, O => O.MapFrom(U => U.Gender.ToString()))
                .ForMember(U => U.BloodType, O => O.MapFrom(U => $"{U.BloodType.Symbol}{U.BloodType.RhFactor}"));
        }
    }
}
