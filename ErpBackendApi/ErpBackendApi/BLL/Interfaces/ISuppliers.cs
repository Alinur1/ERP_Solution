using ErpBackendApi.DAL.Models;

namespace ErpBackendApi.BLL.Interfaces
{
    public interface ISuppliers
    {
        Task<IEnumerable<Supplier>> GetAllSuppliersAsync();
        Task<Supplier> GetSupplierByIdAsync(int id);
        Task<Supplier> AddSupplierAsync(Supplier supplier);
        Task<Supplier> UpdateSupplierAsync(Supplier supplier);
        Task<Supplier> SoftDeleteSupplierAsync(int id);
        Task<Supplier> UndoSoftDeleteSupplierAsync(int id);
        Task<IEnumerable<Supplier>> GetAllDeletedSuppliersAsync();
    }
}
