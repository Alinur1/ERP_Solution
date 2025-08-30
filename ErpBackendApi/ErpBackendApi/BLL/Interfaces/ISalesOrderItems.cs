using ErpBackendApi.DAL.DTOs;
using ErpBackendApi.DAL.Models;

namespace ErpBackendApi.BLL.Interfaces
{
    public interface ISalesOrderItems
    {
        Task<IEnumerable<SalesOrderItemDTO>> GetAllSalesOrderItemAsync();
        Task<SalesOrderItemDTO> GetSalesOrderItemByIdAsync(int id);
        Task<SalesOrderItemDTO> GetSalesOrderItemBySalesOrderIdAsync(int orderId);
        Task<IEnumerable<SalesOrderItem>> AddSalesOrderItemsAsync(IEnumerable<SalesOrderItem> items);
        Task<SalesOrderItem> UpdateSalesOrderItemAsync(SalesOrderItem item);
        Task<SalesOrderItem> SoftDeleteSalesOrderItemAsync(SalesOrderItem item);
        Task<SalesOrderItem> UndoSoftDeleteSalesOrderItemAsync(SalesOrderItem item);
        Task<IEnumerable<SalesOrderItemDTO>> GetAllDeletedSalesOrderItemAsync();
    }
}
