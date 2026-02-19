using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models
{
    public enum NotificationType
    {
        NewBloodRequest = 1,
        DonorAcceptedRequest = 2,
        DonationConfirmed = 3,
        RequestCompleted = 4,
        DonationReported = 5
    }
}
