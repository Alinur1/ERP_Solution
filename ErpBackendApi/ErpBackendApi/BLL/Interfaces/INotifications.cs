using ErpBackendApi.DAL.DTOs;
using ErpBackendApi.DAL.Models;

namespace ErpBackendApi.BLL.Interfaces
{
    public interface INotifications
    {
        Task<IEnumerable<NotificationDTO>> GetAllNotificationsAsync();
        Task<NotificationDTO> GetNotificationByIdAsync(int id);
        Task<Notification> AddNotificationAsync(Notification notification);
        Task<Notification> UpdateNotificationAsync(Notification notification);
        Task<Notification> SoftDeleteNotificationAsync(Notification notification);
        Task<Notification> UndoSoftDeleteNotificationAsync(Notification notification);
    }
}
