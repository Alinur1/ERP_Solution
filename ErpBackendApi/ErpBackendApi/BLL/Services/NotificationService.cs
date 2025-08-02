using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.DTOs;
using ErpBackendApi.DAL.ERPDataContext;
using ErpBackendApi.DAL.Models;
using Microsoft.EntityFrameworkCore;
using static ErpBackendApi.Utilities.Helper.LoggerClass;

namespace ErpBackendApi.BLL.Services
{
    public class NotificationService : INotifications
    {
        private readonly AppDataContext _context;
        public NotificationService(AppDataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<NotificationDTO>> GetAllNotificationsAsync()
        {
            return await
            (
                from n in _context.notifications
                join u in _context.users on n.user_id equals u.id into userGroup
                from u in userGroup.DefaultIfEmpty()
                where n.is_deleted == false
                select new NotificationDTO
                {
                    id = n.id,
                    user_id = u != null ? u.id : null,
                    user_name = u != null && u.is_deleted == false ? u.name : null,
                    title = n.title,
                    message = n.message,
                    is_read = n.is_read,
                    created_at = n.created_at
                }
            ).ToListAsync();
        }

        public async Task<NotificationDTO> GetNotificationByIdAsync(int id)
        {
            return await
            (
                from n in _context.notifications
                join u in _context.users on n.user_id equals u.id into userGroup
                from u in userGroup.DefaultIfEmpty()
                where n.id == id && n.is_deleted == false
                select new NotificationDTO
                {
                    id = n.id,
                    user_id = u != null ? u.id : null,
                    user_name = u != null && u.is_deleted == false ? u.name : null,
                    title = n.title,
                    message = n.message,
                    is_read = n.is_read,
                    created_at = n.created_at
                }
            ).FirstOrDefaultAsync();
        }

        public async Task<Notification> AddNotificationAsync(Notification notification)
        {
            notification.is_read = false;
            notification.created_at = DateTime.UtcNow;
            notification.is_deleted = false;
            notification.deleted_at = null;
            _context.notifications.Add(notification);
            await _context.SaveChangesAsync();
            return notification;
        }

        public async Task<Notification> UpdateNotificationAsync(Notification notification)
        {
            var existingNotification = await _context.notifications.FirstOrDefaultAsync(n => n.id == notification.id && n.is_deleted == false);
            if (existingNotification == null)
            {
                Logger("Unable to update notification. Notification not found.");
                return null;
            }
            existingNotification.user_id = notification.user_id;
            existingNotification.title = notification.title;
            existingNotification.message = notification.message;
            await _context.SaveChangesAsync();
            return existingNotification;
        }

        public async Task<Notification> SoftDeleteNotificationAsync(Notification notification)
        {
            var existingNotification = await _context.notifications.FirstOrDefaultAsync(n => n.id == notification.id && n.is_deleted == false);
            if (existingNotification == null)
            {
                Logger("Unable to delete notification. Notification not found.");
                return null;
            }
            existingNotification.is_deleted = true;
            existingNotification.deleted_at = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return existingNotification;
        }

        public async Task<Notification> UndoSoftDeleteNotificationAsync(Notification notification)
        {
            var existingNotification = await _context.notifications.FirstOrDefaultAsync(n => n.id == notification.id && n.is_deleted == true);
            if (existingNotification == null)
            {
                Logger("Unable to restore deleted notification. Notification not found.");
                return null;
            }
            existingNotification.is_deleted = false;
            existingNotification.deleted_at = null;
            await _context.SaveChangesAsync();
            return existingNotification;
        }
    }
}
