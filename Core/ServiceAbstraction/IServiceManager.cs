using ServiceAbstraction.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction
{
    public interface IServiceManager
    {
        ILookupService LookupService { get; }
        IRequestService RequestService { get; }
        IAuthenticationService AuthenticationService { get; }
        IResponseService ResponseService { get; }
        ICompatibilityService CompatibilityService { get; }
        INotificationService NotificationService { get; }
        IDashboardService DashboardService { get; }
    }
}
