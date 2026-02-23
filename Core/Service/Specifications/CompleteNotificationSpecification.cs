using DomainLayer.Models;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications
{
    internal class CompleteNotificationSpecification : BaseSpecifications<NotificationChild, long>
    {
        public CompleteNotificationSpecification(Guid UserId) : base(C => C.UserId == UserId)
        {
            AddInculde(C => C.NotificationBase);
        }
        public CompleteNotificationSpecification(NotificationQueryParams Params, Guid UserId) : base(C => C.UserId == UserId)
        {
            AddInculde(C => C.NotificationBase);
            ApplyPagination(Params.PageSize, Params.PageNumber);
        }
    }
}
