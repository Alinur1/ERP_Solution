using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.ERPDataContext;
using ErpBackendApi.DAL.Models;
using Microsoft.EntityFrameworkCore;
using static ErpBackendApi.Utilities.Helper.LoggerClass;

namespace ErpBackendApi.BLL.Services
{
    public class CustomerService : ICustomers
    {
        private readonly AppDataContext _context;
        public CustomerService(AppDataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            return await _context.customers
                .Where(c => c.is_deleted == false)
                .Select(c => new Customer
                {
                    id = c.id,
                    name = c.name,
                    email = c.email,
                    phone = c.phone,
                    address = c.address
                }).ToListAsync();
        }

        public async Task<Customer> GetCustomerByIdAsync(int id)
        {
            return await _context.customers
                .Where(c => c.id == id && c.is_deleted == false)
                .Select(c => new Customer
                {
                    id = c.id,
                    name = c.name,
                    email = c.email,
                    phone = c.phone,
                    address = c.address
                }).FirstOrDefaultAsync();
        }

        public async Task<Customer> AddCustomerAsync(Customer customer)
        {
            var existingCustomer = await _context.customers.FirstOrDefaultAsync(c => c.phone == customer.phone && c.is_deleted == false);
            if (existingCustomer != null)
            {
                Logger("A customer with same phone number already exists.");
                return null;
            }
            customer.is_deleted = false;
            customer.deleted_at = null;
            _context.customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task<Customer> UpdateCustomerAsync(Customer customer)
        {
            var existingCustomer = await _context.customers.FirstOrDefaultAsync(c => c.id == customer.id && c.is_deleted == false);
            if (existingCustomer == null)
            {
                Logger("Customer not found to update information.");
                return null;
            }
            existingCustomer.name = customer.name;
            existingCustomer.email = customer.email;
            existingCustomer.phone = customer.phone;
            existingCustomer.address = customer.address;
            _context.customers.Update(existingCustomer);
            await _context.SaveChangesAsync();
            return existingCustomer;
        }

        public async Task<Customer> SoftDeleteCustomerAsync(Customer customer)
        {
            var existingCustomer = await _context.customers.FirstOrDefaultAsync(c => c.id == customer.id && c.is_deleted == false);
            if (existingCustomer == null)
            {
                Logger("Customer not found or already deleted.");
                return null;
            }
            existingCustomer.is_deleted = true;
            existingCustomer.deleted_at = DateTime.UtcNow;
            _context.customers.Update(existingCustomer);
            await _context.SaveChangesAsync();
            return existingCustomer;
        }

        public async Task<Customer> UndoSoftDeleteCustomerAsync(Customer customer)
        {
            var existingCustomer = await _context.customers.FirstOrDefaultAsync(c => c.id == customer.id && c.is_deleted == true);
            if (existingCustomer == null)
            {
                Logger("Unable to restore deleted customer or customer not found.");
                return null;
            }
            existingCustomer.is_deleted = false;
            existingCustomer.deleted_at = null;
            _context.customers.Update(existingCustomer);
            await _context.SaveChangesAsync();
            return existingCustomer;
        }
    }
}
