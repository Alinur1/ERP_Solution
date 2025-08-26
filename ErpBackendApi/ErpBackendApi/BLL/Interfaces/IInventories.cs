using ErpBackendApi.DAL.DTOs;
using ErpBackendApi.DAL.Models;

namespace ErpBackendApi.BLL.Interfaces
{
    public interface IInventories
    {
        Task<IEnumerable<InventoryDTO>> GetAllInventoriesAsync();
        Task<InventoryDTO> GetInventoryByIdAsync(int id);
        Task<Inventory> AddInventoryAsync(Inventory inventory);
        Task<Inventory> UpdateInventoryAsync(Inventory inventory);
        Task<Inventory> SoftDeleteInventoryAsync(Inventory inventory);
        Task<Inventory> UndoSoftDeleteInventoryAsync(Inventory inventory);
        Task<IEnumerable<InventoryDTO>> GetAllDeletedInventoriesAsync();
    }
}
