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
            var operation_AddExpense = await _iExpenses.AddExpenseAsync(expense);
            if (operation_AddExpense == null)
            {
                return NotFound("Unable to add expense.");
            }
            return Ok("Expense added successfully.");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateExpense(Expense expense)
        {
            var operation_UpdateExpense = await _iExpenses.UpdateExpenseAsync(expense);
            if (operation_UpdateExpense == null)
            {
                return NotFound("Unable to update expense.");
            }
            return Ok("Expense updated successfully.");
        }

        [HttpPut("delete")]
        public async Task<IActionResult> SoftDeleteExpense(Expense expense)
        {
            var operation_SoftDeleteExpense  = await _iExpenses.SoftDeleteExpenseAsync(expense);
            if (operation_SoftDeleteExpense == null)
            {
                return NotFound("Unable to delete expense.");
            }
            return Ok("Expense deleted successfully.");
        }

        [HttpPut("undo-delete")]
        public async Task<IActionResult> UndoSoftDeleteExpense(Expense expense)
        {
            var operation_UndoSoftDeleteExpense = await _iExpenses.UndoSoftDeleteExpenseAsync(expense);
            if (operation_UndoSoftDeleteExpense == null)
            {
                return NotFound("Unable to restore deleted expense.");
            }
            return Ok("Expense restored successfully.");
        }
    }
}
