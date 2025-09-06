using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ErpBackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpenseController : ControllerBase
    {
        private readonly IExpenses _iExpenses;
        public ExpenseController(IExpenses iExpenses)
        {
            _iExpenses = iExpenses;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllExpenses()
        {
            var operation_GetAllExpenses = await _iExpenses.GetAllExpenseAsync();
            return Ok(operation_GetAllExpenses);
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetExpenseById(int id)
        {
            var operation_GetExpenseById = await _iExpenses.GetExpenseByIdAsync(id);
            if (operation_GetExpenseById == null)
            {
                return NotFound("Expense not found.");
            }
            return Ok(operation_GetExpenseById);
        }

        [HttpPost]
        public async Task<IActionResult> AddExpense(Expense expense)
        {
            try
            {
                var operation_AddExpense = await _iExpenses.AddExpenseAsync(expense);
                return Ok("Expense added successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateExpense(Expense expense)
        {
            try
            {
                var operation_UpdateExpense = await _iExpenses.UpdateExpenseAsync(expense);
                return Ok("Expense updated successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("delete")]
        public async Task<IActionResult> SoftDeleteExpense(Expense expense)
        {
            try
            {
                var operation_SoftDeleteExpense = await _iExpenses.SoftDeleteExpenseAsync(expense);
                return Ok("Expense deleted successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("undo-delete")]
        public async Task<IActionResult> UndoSoftDeleteExpense(Expense expense)
        {
            try
            {
                var operation_UndoSoftDeleteExpense = await _iExpenses.UndoSoftDeleteExpenseAsync(expense);
                return Ok("Expense restored successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
