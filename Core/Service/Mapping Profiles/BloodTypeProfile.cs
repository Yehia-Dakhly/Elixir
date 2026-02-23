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
    internal class BloodTypeProfile : Profile
    {
        public BloodTypeProfile()
        {
            CreateMap<BloodTypes, BloodTypeDTo>().ReverseMap();
        }
    }
}
