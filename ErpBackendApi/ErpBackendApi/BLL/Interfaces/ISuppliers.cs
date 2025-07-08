using ErpBackendApi.DAL.Models;

namespace ErpBackendApi.BLL.Interfaces
{
    public interface ISuppliers
    {
        Task<IEnumerable<Supplier>> GetAllSuppliersAsync();
        Task<Supplier> GetSupplierByIdAsync(int id);
        Task<Supplier> AddSupplierAsync(Supplier supplier);
        Task<Supplier> UpdateSupplierAsync(Supplier supplier);
        Task<Supplier> SoftDeleteSupplierAsync(Supplier supplier);
        Task<Supplier> UndoSoftDeleteSupplierAsync(Supplier supplier);
    }
}
