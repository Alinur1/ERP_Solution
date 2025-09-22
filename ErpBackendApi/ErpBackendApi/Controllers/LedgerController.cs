using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ErpBackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LedgerController : ControllerBase
    {
        private readonly ILedgers _iLedger;
        public LedgerController(ILedgers iLedger)
        {
            _iLedger = iLedger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLedgers()
        {
            try
            {
                var operation_GetAllLedgers = await _iLedger.GetAllLedgersAsync();
                return Ok(operation_GetAllLedgers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving ledgers: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLedgerById(int id)
        {
            try
            {
                var operation_GetLedgerById = await _iLedger.GetLedgerByIdAsync(id);
                if (operation_GetLedgerById == null)
                {
                    return NotFound("Ledger entry not found.");
                }
                return Ok(operation_GetLedgerById);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving ledger: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddLedger(Ledger ledger)
        {
            try
            {
                var operation_AddLedger = await _iLedger.AddLedgerAsync(ledger);
                return Ok("Ledger entry added successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error adding ledger entry: {ex.Message}");
            }
        }
    }
}
