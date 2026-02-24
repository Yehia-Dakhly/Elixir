using AutoMapper;
using DomainLayer.Contracts;
using DomainLayer.Exceptions;
using DomainLayer.Exceptions.NotFoundExceptions;
using DomainLayer.Models;
using Microsoft.Extensions.Logging;
using Service.Specifications;
using ServiceAbstraction;
using Shared;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class NotificationService(
        IUnitOfWork _unitOfWork,
        IMapper _mapper,
        ILogger<NotificationService> _logger
        ) : INotificationService
    {
        public async Task<PaginatedResult<NotificationDTo>> GetAllNotificationAsync(NotificationQueryParams Params, Guid UserId)
        {
            var ChildRepo = _unitOfWork.GetRepository<NotificationChild, long>();
            var Specification = new CompleteNotificationSpecification(Params, UserId);
            var Notifications = await ChildRepo.GetAllAsync(Specification);
            var Count = await ChildRepo.CountAsync(Specification);
            return new PaginatedResult<NotificationDTo>
                (Params.PageSize,
                Params.PageNumber,Count,
                _mapper.Map<IEnumerable<NotificationDTo>>(Notifications));    
        }

        public async Task<int> GetAllNotificationCount(Guid UserId)
        {
            var Repo = _unitOfWork.GetRepository<NotificationChild, long>();
            var Specification = new CompleteNotificationSpecification(UserId);
            var Notifications = await Repo.GetAllAsync(Specification);
            return Notifications.Count();
        }

        public async Task ReadAllNotificationAsync(Guid UserId)
        {
            var Repo = _unitOfWork.GetRepository<NotificationChild, long>();
            var Specification = new CompleteNotificationSpecification(UserId);
            var Notifications = await Repo.GetAllAsync(Specification);
            foreach (var item in Notifications)
            {
                item.IsRead = true;
                Repo.Update(item);
            }
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ReadNotificationAsync(Guid UserId, long NotificationId)
        {
            var Repo = _unitOfWork.GetRepository<NotificationChild, long>();
            var Notification = await Repo.GetByIdAsync(NotificationId) ?? throw new NotificationNotFound(NotificationId);
            if (UserId == Notification.UserId)
            {
                Notification.IsRead = true;
                Repo.Update(Notification);
                await _unitOfWork.SaveChangesAsync();
            }
            else
            {
                _logger.LogWarning("User with id {UserId} tried to read notification with id {NotificationId} that does not belong to them", UserId, NotificationId);   
                throw new ForbiddenException("غير مسموح لك بقراءة هذا الإشعار");
            }
        }
    }
}
