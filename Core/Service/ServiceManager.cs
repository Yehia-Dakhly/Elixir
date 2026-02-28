using ServiceAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ServiceManager
        (Func<ILookupService> LookupFactory,
        Func<IRequestService> RequestFactory,
        Func<IAuthenticationService> AuthenticationFactory,
        Func<IResponseService> ResponseFactory,
        Func<ICompatibilityService> CompatibilityFactory,
        Func<INotificationService> NotificationFactory,
        Func<IDashboardService> DashboardFactory

        ) : IServiceManager
    {
        public ILookupService LookupService => LookupFactory.Invoke();
        public IRequestService RequestService => RequestFactory.Invoke();
        public IAuthenticationService AuthenticationService => AuthenticationFactory.Invoke();
        public IResponseService ResponseService => ResponseFactory.Invoke();
        public ICompatibilityService CompatibilityService => CompatibilityFactory.Invoke();
        public INotificationService NotificationService => NotificationFactory.Invoke();
        public IDashboardService DashboardService => DashboardFactory.Invoke();
    }
}
