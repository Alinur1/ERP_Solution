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
            var operation_GetAllLedgers = await _iLedger.GetAllLedgersAsync();
            return Ok(operation_GetAllLedgers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLedgerById(int id)
        {
            var operation_GetLedgerById = await _iLedger.GetLedgerByIdAsync(id);
            if (operation_GetLedgerById == null)
            {
                return NotFound("Ledger information not found. Invalid ledger ID.");
            }
            return Ok(operation_GetLedgerById);
        }

        [HttpPost]
        public async Task<IActionResult> AddLedger(Ledger ledger)
        {
            var operation_AddLedger = await _iLedger.AddLedgerAsync(ledger);
            if (operation_AddLedger == null)
            {
                return NotFound("Unable to add ledger information. Something went wrong.");
            }
            return Ok("Ledger information added successfully.");
        }
    }
}
