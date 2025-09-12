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
        private readonly IAccounts _iAccount;
        public AccountController(IAccounts iAccount)
        {
            _iAccount = iAccount;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAccount()
        {
            var operation_GetAllAccount = await _iAccount.GetAllAccountsAsync();
            return Ok(operation_GetAllAccount);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccountById(int id)
        {
            var operation_GetAccountById = await _iAccount.GetAccountByIdAsync(id);
            if (operation_GetAccountById == null)
            {
                return BadRequest("Account not found.");
            }
            return Ok(operation_GetAccountById);
        }

        [HttpPost]
        public async Task<IActionResult> AddAccount(Account account)
        {
            try
            {
                var operation_AddAccount = await _iAccount.AddAccountAsync(account);
                return Ok("Account details added successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateAccount(Account account)
        {
            try
            {
                var operation_UpdateAccount = await _iAccount.UpdateAccountAsync(account);
                return Ok("Account details updated successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("delete")]
        public async Task<IActionResult> SoftDeleteAccount(Account account)
        {
            try
            {
                var operation_DeleteAccount = await _iAccount.SoftDeleteAccountAsync(account);
                return Ok("Account details deleted successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("undo-delete")]
        public async Task<IActionResult> UndoSoftDeleteAccount(Account account)
        {
            try
            {
                var operation_UndoDeleteAccount = await _iAccount.UndoSoftDeleteAccountAsync(account);
                return Ok("Deleted account details restored successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
