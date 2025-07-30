using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ErpBackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccount _iAccount;
        public AccountController(IAccount iAccount)
        {
            _iAccount = iAccount;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAccount()
        {
            var operation_GetAllAccount = await _iAccount.GetAllAccountsAsync();
            return Ok(operation_GetAllAccount);
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetAccountById(int id)
        {
            var operation_GetAccountById = await _iAccount.GetAccountByIdAsync(id);
            if (operation_GetAccountById == null)
            {
                return NotFound("Account not found.");
            }
            return Ok(operation_GetAccountById);
        }

        [HttpPost]
        public async Task<IActionResult> AddAccount(Account account)
        {
            var operation_AddAccount = await _iAccount.AddAccountAsync(account);
            if (operation_AddAccount == null)
            {
                return NotFound("Something went wrong. Unable to update account.");
            }
            return Ok("Account details added successfully.");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAccount(Account account)
        {
            var operation_UpdateAccount = await _iAccount.UpdateAccountAsync(account);
            if (operation_UpdateAccount == null)
            {
                return NotFound("Unable to update account details. Account not found.");
            }
            return Ok("Account details updated successfully.");
        }

        [HttpPut("delete")]
        public async Task<IActionResult> SoftDeleteAccount(Account account)
        {
            var operation_DeleteAccount = await _iAccount.SoftDeleteAccountAsync(account);
            if (operation_DeleteAccount == null)
            {
                return NotFound("Unable to delete account details. Account not found.");
            }
            return Ok("Account details deleted successfully.");
        }

        [HttpPut("undo-delete")]
        public async Task<IActionResult> UndoSoftDeleteAccount(Account account)
        {
            var operation_UndoDeleteAccount = await _iAccount.UndoSoftDeleteAccountAsync(account);
            if (operation_UndoDeleteAccount == null)
            {
                return NotFound("Unable to restore account details. Account not found.");
            }
            return Ok("Deleted account details restored successfully.");
        }
    }
}
