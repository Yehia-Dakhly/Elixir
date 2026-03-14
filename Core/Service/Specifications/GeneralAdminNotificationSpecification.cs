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
    internal class GeneralAdminNotificationSpecification : BaseSpecifications<NotificationBase, long>
    {
        public GeneralAdminNotificationSpecification(AdminNotificationQueryParams queryParams) 
            : base(N => (string.IsNullOrEmpty(queryParams.SearchTitle) || N.Title.Contains(queryParams.SearchTitle))
                  && (string.IsNullOrEmpty(queryParams.SearchTitle) || N.Body.Contains(queryParams.SearchTitle))
                  )
        {
            AddOrderByDescending(N => N.SendAt);
            AddInculde(N => N.NotificationChilderns);
            ApplyPagination(queryParams.PageSize, queryParams.PageIndex);
        }
    }
}
