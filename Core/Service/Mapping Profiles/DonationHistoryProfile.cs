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
    public class DonationHistoryProfile : Profile
    {
        public DonationHistoryProfile()
        {
            CreateMap<DonationHistory, DonationHistoryDTo>()
                .ForMember(D => D.DonationCategoryName, O => O.MapFrom(D => D.DonationCategory.NameAr))
                .ForMember(D => D.DonationDate, O => O.MapFrom(D => D.DonationDate));
        }
    }
}
