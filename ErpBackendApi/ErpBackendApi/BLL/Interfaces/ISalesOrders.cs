using ErpBackendApi.DAL.DTOs;
using ErpBackendApi.DAL.Models;

namespace ErpBackendApi.BLL.Interfaces
{
    public interface ISalesOrders
    {
        Task<IEnumerable<SalesOrderDTO>> GetAllSalesOrderAsync();
        Task<SalesOrderDTO> GetSalesOrderByIdAsync(int id);
        Task<SalesOrderDTO> GetSalesOrderByCustomerIdAsync(int customerId);
        Task<SalesOrder> AddSalesOrderAsync(SalesOrder salesOrder);
        Task<SalesOrder> UpdateSalesOrderAsync(SalesOrder salesOrder);
        Task<SalesOrder> SoftDeleteSalesOrderAsync(SalesOrder salesOrder);
        Task<SalesOrder> UndoSoftDeleteSalesOrderAsync(SalesOrder salesOrder);
    }
}
