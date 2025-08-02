using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ErpBackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotifications _iNotifications;
        public NotificationController(INotifications iNotifications)
        {
            _iNotifications = iNotifications;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllNotifications()
        {
            var not = await _iNotifications.GetAllNotificationsAsync();
            return Ok(not);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNotificationById(int id)
        {
            var not = await _iNotifications.GetNotificationByIdAsync(id);
            if (not == null)
            {
                return NotFound("Notification not found.");
            }
            return Ok(not);
        }

        [HttpPost]
        public async Task<IActionResult> AddNotification(Notification notification)
        {
            var not = await _iNotifications.AddNotificationAsync(notification);
            if (not == null)
            {
                return NotFound("Unable to add notification. Something went wrong.");
            }
            return Ok("Notification added successfully.");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateNotification(Notification notification)
        {
            var not = await _iNotifications.UpdateNotificationAsync(notification);
            if (not == null)
            {
                return NotFound("Unable to update notification. Notification not found.");
            }
            return Ok("Notification updated successfully.");
        }

        [HttpPut("delete")]
        public async Task<IActionResult> SoftDeleteNotification(Notification notification)
        {
            var not = await _iNotifications.SoftDeleteNotificationAsync(notification);
            if (not == null)
            {
                return NotFound("Unable to delete notification. Notification not found.");
            }
            return Ok("Notification deleted successfully.");
        }

        [HttpPut("undo-delete")]
        public async Task<IActionResult> UndoSoftDeleteNotification(Notification notification)
        {
            var not = await _iNotifications.UndoSoftDeleteNotificationAsync(notification);
            if (not == null)
            {
                return NotFound("Unable to restore deleted notification. Notification not found.");
            }
            return Ok("Notification restored successfully.");
        }
    }
}
