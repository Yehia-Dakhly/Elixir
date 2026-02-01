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
    public class BloodRequestProfile : Profile
    {
        public BloodRequestProfile()
        {
            CreateMap<CreateBloodRequestDTo, BloodRequests>().ReverseMap();
            CreateMap<BloodRequests, BloodRequestDTo>()
                .ForMember(Q => Q.Status, O => O.MapFrom(Q => Q.Status.ToString()))
                .ForMember(Q => Q.RequesterName, O => O.MapFrom(Q => Q.Requester.FullName))
                .ForMember(Q => Q.DonationCategoryAr, O => O.MapFrom(Q => Q.DonationCategory.NameAr))
                .ForMember(Q => Q.CityAr, O => O.MapFrom(Q => Q.City.NameAr))
                .ForMember(Q => Q.CityEn, O => O.MapFrom(Q => Q.City.NameEn))
                .ForMember(Q => Q.RequiredBloodType, O => O.MapFrom(Q => $"{Q.RequiredBloodType.Symbol}{Q.RequiredBloodType.RhFactor}"));
        }
    }
}
