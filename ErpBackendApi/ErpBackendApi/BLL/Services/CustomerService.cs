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
            var existingCustomerEmail = await _context.customers.FirstOrDefaultAsync(c => c.email == customer.email && c.is_deleted == false);

            if (existingCustomer != null)
            {
                Logger("A customer with same phone number already exists.");
                throw new InvalidOperationException("A customer with same phone number already exists.");
            }

            if (string.IsNullOrWhiteSpace(customer.name) || string.IsNullOrWhiteSpace(customer.phone))
            {
                Logger("Customer name and phone number are required.");
                throw new InvalidOperationException("Customer name and phone number are required.");
            }

            if (!string.IsNullOrWhiteSpace(customer.email))
            {                
                if (existingCustomerEmail != null)
                {
                    Logger("A customer with same email already exists.");
                    throw new InvalidOperationException("A customer with same email already exists.");
                }
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
            var existingCustomerPhone = await _context.customers.FirstOrDefaultAsync(c => c.phone == customer.phone && c.id != customer.id && c.is_deleted == false);
            var existingCustomerEmail = await _context.customers.FirstOrDefaultAsync(c => c.email == customer.email && c.id != customer.id && c.is_deleted == false);

            if (existingCustomer == null)
            {
                Logger("Customer not found to update information.");
                throw new InvalidOperationException("Customer not found to update information.");
            }

            if (existingCustomer.phone != customer.phone)
            {
                if (existingCustomerPhone != null)
                {
                    Logger("Error! Duplicate phone number for a customer.");
                    throw new InvalidOperationException("Error! Duplicate phone number for a customer.");
                }
            }

            if (string.IsNullOrWhiteSpace(customer.name) || string.IsNullOrWhiteSpace(customer.phone))
            {
                Logger("Customer name and phone number are required.");
                throw new InvalidOperationException("Customer name and phone number are required.");
            }

            if (!string.IsNullOrWhiteSpace(customer.email) && existingCustomer.email != customer.email)
            {                
                if (existingCustomerEmail != null)
                {
                    Logger("Error! Duplicate email for a customer.");
                    throw new InvalidOperationException("Error! Duplicate email for a customer.");
                }
            }

            existingCustomer.name = customer.name;
            existingCustomer.email = customer.email;
            existingCustomer.phone = customer.phone;
            existingCustomer.address = customer.address;
            await _context.SaveChangesAsync();
            return existingCustomer;
        }

        public async Task<Customer> SoftDeleteCustomerAsync(Customer customer)
        {
            var existingCustomer = await _context.customers.FirstOrDefaultAsync(c => c.id == customer.id && c.is_deleted == false);
            if (existingCustomer == null)
            {
                Logger("Customer not found or already deleted.");
                throw new InvalidOperationException("Customer not found or already deleted.");
            }
            existingCustomer.is_deleted = true;
            existingCustomer.deleted_at = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return existingCustomer;
        }

        public async Task<Customer> UndoSoftDeleteCustomerAsync(Customer customer)
        {
            var existingCustomer = await _context.customers.FirstOrDefaultAsync(c => c.id == customer.id && c.is_deleted == true);
            if (existingCustomer == null)
            {
                Logger("Unable to restore deleted customer or customer not found.");
                throw new InvalidOperationException("Unable to restore deleted customer or customer not found.");
            }
            existingCustomer.is_deleted = false;
            existingCustomer.deleted_at = null;
            await _context.SaveChangesAsync();
            return existingCustomer;
        }

        public async Task<IEnumerable<Customer>> GetAllDeletedCustomerAsync()
        {
            return await _context.customers
                .Where(c => c.is_deleted == true)
                .Select(c => new Customer
                {
                    id = c.id,
                    name = c.name,
                    email = c.email,
                    phone = c.phone,
                    address = c.address
                }).ToListAsync();
        }
    }
}
