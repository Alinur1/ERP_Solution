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
                return BadRequest("Transaction not found.");
            }
            return Ok(operation_GetTransactionById);
        }

        [HttpPost]
        public async Task<IActionResult> AddTransaction(Transaction transaction)
        {
            try
            {
                var operation_AddTransaction = await _iTransaction.AddTransactionAsync(transaction);
                return Ok("Transaction added successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTransaction(Transaction transaction)
        {
            try
            {
                var operation_UpdateTransaction = await _iTransaction.UpdateTransactionAsync(transaction);
                return Ok("Transaction updated successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("delete")]
        public async Task<IActionResult> SoftDeleteTransaction(Transaction transaction)
        {
            try
            {
                var operation_SoftDeleteTransaction = await _iTransaction.SoftDeleteTransactionAsync(transaction);
                return Ok("Transaction information deleted successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("undo-delete")]
        public async Task<IActionResult> UndoSoftDeleteTransaction(Transaction transaction)
        {
            try
            {
                var operation_UndoSoftDeleteTransaction = await _iTransaction.UndoSoftDeleteTransactionAsync(transaction);
                return Ok("Transaction information restored successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
