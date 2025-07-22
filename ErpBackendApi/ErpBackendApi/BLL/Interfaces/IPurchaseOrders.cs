using ErpBackendApi.DAL.DTOs;
using ErpBackendApi.DAL.Models;

namespace ErpBackendApi.BLL.Interfaces
{
    public interface IPurchaseOrders
    {
        Task<IEnumerable<PurchaseOrderDTO>> GetAllPurchaseOrdersAsync();
        Task<PurchaseOrderDTO> GetPurchaseOrderByIdAsync(int id);
        Task<PurchaseOrderDTO> GetPurchaseOrderBySupplierIdAsync(int supplierId);
        Task<PurchaseOrder> AddPurchaseOrderAsync(PurchaseOrder purchaseOrder);
        Task<PurchaseOrder> UpdatePurchaseOrderAsync(PurchaseOrder purchaseOrder);
        Task<PurchaseOrder> SoftDeletePurchaseOrderAsync(PurchaseOrder purchaseOrder);
        Task<PurchaseOrder> UndoSoftDeletePurchaseOrderAsync(PurchaseOrder purchaseOrder);

    }
}
