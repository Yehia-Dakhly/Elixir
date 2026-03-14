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
    public class NotificationsProfile : Profile
    {
        public NotificationsProfile()
        {
            CreateMap<NotificationChild, NotificationDTo>()
                .ForMember(N => N.CreatedAt, O => O.MapFrom(N => N.NotificationBase.SendAt))
                .ForMember(N => N.Title, O => O.MapFrom(N => N.NotificationBase.Title))
                .ForMember(N => N.BloodRequestId, O => O.MapFrom(N => N.NotificationBase.BloodRequestId))
                .ForMember(N => N.Body, O => O.MapFrom(N => N.NotificationBase.Body))
                .ForMember(N => N.Data, O => O.MapFrom(N => N.NotificationBase.Data))
                .ForMember(N => N.NotificationType, O => O.MapFrom(N => N.NotificationBase.NotificationType));

            CreateMap<NotificationBase, AdminNotificationDTo>()
                .ForMember(N => N.SendedToCount, O => O.MapFrom(N => N.NotificationChilderns.Count))
                .ForMember(N => N.NotificationType, O => O.MapFrom(N => N.NotificationType))
                .ForMember(N => N.Title, O => O.MapFrom(N => N.Title))
                .ForMember(N => N.BloodRequestId, O => O.MapFrom(N => N.BloodRequestId))
                .ForMember(N => N.Body, O => O.MapFrom(N => N.Body))
                .ForMember(N => N.SendAt, O => O.MapFrom(N => N.SendAt));
                
        }
    }
}
