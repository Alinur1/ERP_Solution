using ErpBackendApi.DAL.DTOs;
using ErpBackendApi.DAL.Models;

namespace ErpBackendApi.BLL.Interfaces
{
    public interface IPurchaseOrderItems
    {
        Task<IEnumerable<PurchaseOrderItemDTO>> GetAllPurchaseOrderItemsAsync();
        Task<PurchaseOrderItemDTO> GetPurchaseOrderItemByIdAsync(int id);
        Task<PurchaseOrderItemDTO> GetPurchaseOrderItemByPurchaseOrderIdAsync(int orderId);
        Task<IEnumerable<PurchaseOrderItem>> AddPurchaseOrderItemAsync(IEnumerable<PurchaseOrderItem> items);
        Task<PurchaseOrderItem> UpdatePurchaseOrderItemAsync(PurchaseOrderItem item);
        Task<PurchaseOrderItem> SoftDeletePurchaseOrderItemAsync(PurchaseOrderItem item);
        Task<PurchaseOrderItem> UndoSoftDeletePurchaseOrderItemAsync(PurchaseOrderItem item);
        Task<IEnumerable<PurchaseOrderItemDTO>> GetAllDeletedPurchaseOrderItemsAsync();
    }
}
