using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ErpBackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomers _iCustomer;
        public CustomerController(ICustomers iCustomer)
        {
            _iCustomer = iCustomer;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCustomers()
        {
            var operation_GetAllCustomers = await _iCustomer.GetAllCustomersAsync();
            return Ok(operation_GetAllCustomers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            var operation_GetCustomerById = await _iCustomer.GetCustomerByIdAsync(id);
            if (operation_GetCustomerById == null)
            {
                return NotFound("Customer not found.");
            }
            return Ok(operation_GetCustomerById);
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomer(Customer customer)
        {
            var operation_AddCustomer = await _iCustomer.AddCustomerAsync(customer);
            if (operation_AddCustomer == null)
            {
                return NotFound("A customer with same phone number already exists.");
            }
            return Ok("Customer added successfully.");
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateCustomer(Customer customer)
        {
            var operation_UpdateCustomer = await _iCustomer.UpdateCustomerAsync(customer);
            if (operation_UpdateCustomer == null)
            {
                return NotFound("Unable to update customer information.");
            }
            return Ok("Customer information updated successfully.");
        }

        [HttpPut("delete")]
        public async Task<IActionResult> SoftDeleteCustomer(Customer customer)
        {
            var operation_SoftDeleteCustomer = await _iCustomer.SoftDeleteCustomerAsync(customer);
            if (operation_SoftDeleteCustomer == null)
            {
                return NotFound("Unable to delete customer data.");
            }
            return Ok("Customer information deleted successfully.");
        }

        [HttpPut("undo-delete")]
        public async Task<IActionResult> UndoSoftDeleteCustomer(Customer customer)
        {
            var operation_UndoSoftDeleteCustomer = await _iCustomer.UndoSoftDeleteCustomerAsync(customer);
            if (operation_UndoSoftDeleteCustomer == null)
            {
                return NotFound("Unable to restore deleted customer data.");
            }
            return Ok("Customer information restored successfully.");
        }
    }
}
