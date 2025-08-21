using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.ERPDataContext;
using ErpBackendApi.DAL.Models;
using Microsoft.EntityFrameworkCore;
using static ErpBackendApi.Utilities.Helper.LoggerClass;

namespace ErpBackendApi.BLL.Services
{
    public class SupplierService : ISuppliers
    {
        private readonly AppDataContext _context;
        public SupplierService(AppDataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Supplier>> GetAllSuppliersAsync()
        {
            return await _context.suppliers
                .Where(s => s.is_deleted == false)
                .Select(s => new Supplier
                {
                    id = s.id,
                    company_name = s.company_name,
                    contact_person_name = s.contact_person_name,
                    phone = s.phone,
                    email = s.email,
                    address = s.address,
                }).ToListAsync();
        }

        public async Task<Supplier> GetSupplierByIdAsync(int id)
        {
            return await _context.suppliers
                .Where(s => s.id == id && s.is_deleted == false)
                .Select(s => new Supplier
                {
                    id = s.id,
                    company_name = s.company_name,
                    contact_person_name = s.contact_person_name,
                    phone = s.phone,
                    email = s.email,
                    address = s.address,
                }).FirstOrDefaultAsync();
        }

        public async Task<Supplier> AddSupplierAsync(Supplier supplier)
        {
            var existingSupplier = await _context.suppliers.FirstOrDefaultAsync(s => s.phone == supplier.phone && s.is_deleted == false);
            if (existingSupplier != null)
            {
                Logger("Tried to create supplier with same phone number.");
                throw new InvalidOperationException("Tried to create supplier with same phone number.");
            }
            if (!string.IsNullOrWhiteSpace(supplier.email))
            {
                supplier.email = supplier.email.Trim();
                var existingSupplierEmail = await _context.suppliers.FirstOrDefaultAsync(s => s.email == supplier.email && s.is_deleted == false);
                if (existingSupplierEmail != null)
                {
                    Logger("This supplier email is already in use.");
                    throw new InvalidOperationException("This supplier email is already in use.");
                }
            }
            supplier.is_deleted = false;
            supplier.deleted_at = null;
            _context.suppliers.Add(supplier);
            await _context.SaveChangesAsync();
            return supplier;
        }

        public async Task<Supplier> UpdateSupplierAsync(Supplier supplier)
        {
            var existingSupplier = await _context.suppliers.FirstOrDefaultAsync(s => s.id == supplier.id && s.is_deleted == false);
            if (existingSupplier == null)
            {
                Logger("Supplier not found. Unable to update supplier information.");
                throw new InvalidOperationException("Supplier not found. Unable to update supplier information.");
            }
            if (!string.IsNullOrWhiteSpace(supplier.email) && existingSupplier.email != supplier.email)
            {
                supplier.email = supplier.email.Trim();
                var existingSupplierEmail = await _context.suppliers.FirstOrDefaultAsync(s => s.email == supplier.email && s.id != supplier.id && s.is_deleted == false);
                if (existingSupplierEmail != null)
                {
                    Logger("This supplier email is already in use.");
                    throw new InvalidOperationException("This supplier email is already in use.");
                }
            }
            if (existingSupplier.phone != supplier.phone)
            {
                var existingSupplierPhone = await _context.suppliers.FirstOrDefaultAsync(s => s.phone == supplier.phone && s.id != supplier.id && s.is_deleted == false);
                if (existingSupplierPhone != null)
                {
                    Logger("This supplier phone number is already in use.");
                    throw new InvalidOperationException("This supplier phone number is already in use.");
                }
            }
            existingSupplier.company_name = supplier.company_name;
            existingSupplier.contact_person_name = supplier.contact_person_name;
            existingSupplier.phone = supplier.phone;
            existingSupplier.email = supplier.email;
            existingSupplier.address = supplier.address;
            await _context.SaveChangesAsync();
            return existingSupplier;
        }

        public async Task<Supplier> SoftDeleteSupplierAsync(Supplier supplier)
        {
            var existingSupplier = await _context.suppliers.FirstOrDefaultAsync(s => s.id == supplier.id && s.is_deleted == false);
            if (existingSupplier == null)
            {
                Logger("Supplier not found. Unable to delete supplier information.");
                throw new InvalidOperationException("Supplier not found. Unable to delete supplier information.");
            }
            existingSupplier.is_deleted = true;
            existingSupplier.deleted_at = DateTime.UtcNow;
            _context.suppliers.Update(existingSupplier);
            await _context.SaveChangesAsync();
            return existingSupplier;
        }

        public async Task<Supplier> UndoSoftDeleteSupplierAsync(Supplier supplier)
        {
            var existingSupplier = await _context.suppliers.FirstOrDefaultAsync(s => s.id == supplier.id && s.is_deleted == true);
            if (existingSupplier == null)
            {
                Logger("Deleted supplier not found. Unable to restore deleted supplier information.");
                throw new InvalidOperationException("Deleted supplier not found. Unable to restore deleted supplier information.");
            }
            existingSupplier.is_deleted = false;
            existingSupplier.deleted_at = null;
            _context.suppliers.Update(existingSupplier);
            await _context.SaveChangesAsync();
            return existingSupplier;
        }

        public async Task<IEnumerable<Supplier>> GetAllDeletedSuppliersAsync()
        {
            return await _context.suppliers
                .Where(s => s.is_deleted == true)
                .Select(s => new Supplier
                {
                    id = s.id,
                    company_name = s.company_name,
                    contact_person_name = s.contact_person_name,
                    phone = s.phone,
                    email = s.email,
                    address = s.address,
                    deleted_at = s.deleted_at,
                }).ToListAsync();
        }
    }
}
