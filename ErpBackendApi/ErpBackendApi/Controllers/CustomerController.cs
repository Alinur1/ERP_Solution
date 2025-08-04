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
            try
            {
                var operation_AddCustomer = await _iCustomer.AddCustomerAsync(customer);
                return Ok("Customer added successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateCustomer(Customer customer)
        {
            try
            {
                var operation_UpdateCustomer = await _iCustomer.UpdateCustomerAsync(customer);
                return Ok("Customer information updated successfully.");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("delete")]
        public async Task<IActionResult> SoftDeleteCustomer(Customer customer)
        {
            try
            {
                var operation_SoftDeleteCustomer = await _iCustomer.SoftDeleteCustomerAsync(customer);
                return Ok("Customer information deleted successfully.");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("undo-delete")]
        public async Task<IActionResult> UndoSoftDeleteCustomer(Customer customer)
        {
            try
            {
                var operation_UndoSoftDeleteCustomer = await _iCustomer.UndoSoftDeleteCustomerAsync(customer);
                return Ok("Customer information restored successfully.");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
