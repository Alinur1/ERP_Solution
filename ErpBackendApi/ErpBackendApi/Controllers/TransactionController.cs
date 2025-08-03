using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ErpBackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactions _iTransaction;
        public TransactionController(ITransactions iTransaction)
        {
            _iTransaction = iTransaction;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTransactions()
        {
            var operation_GetAllTransactions = await _iTransaction.GetAllTransactionsAsync();
            return Ok(operation_GetAllTransactions);
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetTransactionById(int id)
        {
            var operation_GetTransactionById = await _iTransaction.GetTransactionByIdAsync(id);
            if (operation_GetTransactionById == null)
            {
                return NotFound("Transaction not found.");
            }
            return Ok(operation_GetTransactionById);
        }

        [HttpPost]
        public async Task<IActionResult> AddTransaction(Transaction transaction)
        {
            var operation_AddTransaction = await _iTransaction.AddTransactionAsync(transaction);
            if (operation_AddTransaction == null)
            {
                return NotFound("Unable to add transaction information. Something went wrong.");
            }
            return Ok("Transaction added successfully.");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTransaction(Transaction transaction)
        {
            var operation_UpdateTransaction = await _iTransaction.UpdateTransactionAsync(transaction);
            if (operation_UpdateTransaction == null)
            {
                return NotFound("Unable to update transaction information. Transaction not found.");
            }
            return Ok("Transaction updated successfully.");
        }

        [HttpPut("delete")]
        public async Task<IActionResult> SoftDeleteTransaction(Transaction transaction)
        {
            var operation_SoftDeleteTransaction = await _iTransaction.SoftDeleteTransactionAsync(transaction);
            if (operation_SoftDeleteTransaction == null)
            {
                return NotFound("Unable to delete transaction information. Transaction not found.");
            }
            return Ok("Transaction information deleted successfully.");
        }

        [HttpPut("undo-delete")]
        public async Task<IActionResult> UndoSoftDeleteTransaction(Transaction transaction)
        {
            var operation_UndoSoftDeleteTransaction = await _iTransaction.UndoSoftDeleteTransactionAsync(transaction);
            if (operation_UndoSoftDeleteTransaction == null)
            {
                return NotFound("Unable to restore deleted transaction information. Transaction not found.");
            }
            return Ok("Transaction information restored successfully.");
        }
    }
}
