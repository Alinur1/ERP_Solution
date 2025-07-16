using ErpBackendApi.DAL.DTOs;
using ErpBackendApi.DAL.Models;

namespace ErpBackendApi.BLL.Interfaces
{
    public interface ISalesOrderItems
    {
        Task<IEnumerable<SalesOrderItemDTO>> GetAllSalesOrderItemAsync();
        Task<SalesOrderItemDTO> GetSalesOrderItemByIdAsync(int id);
        Task<SalesOrderItemDTO> GetSalesOrderItemBySalesOrderIdAsync(int orderId);
        Task<SalesOrderItem> AddSalesOrderItemAsync(SalesOrderItem item);
        Task<SalesOrderItem> UpdateSalesOrderItemAsync(SalesOrderItem item);
        Task<SalesOrderItem> SoftDeleteSalesOrderItemAsync(SalesOrderItem item);
        Task<SalesOrderItem> UndoSoftDeleteSalesOrderItemAsync(SalesOrderItem item);
    }
}
