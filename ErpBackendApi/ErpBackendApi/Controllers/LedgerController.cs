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
        private readonly ILedger _iLedger;
        public LedgerController(ILedger iLedger)
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

        [HttpPut]
        public async Task<IActionResult> UpdateLedger(Ledger ledger)
        {
            var operation_UpdateLedger = await _iLedger.UpdateLedgerAsync(ledger);
            if (operation_UpdateLedger == null)
            {
                return NotFound("Unable to update ledger information. Invalid ledger ID.");
            }
            return Ok("Ledger information updated successfully.");
        }

        [HttpPut("delete")]
        public async Task<IActionResult> SoftDeleteLedger(Ledger ledger)
        {
            var operation_SoftDeleteLedger = await _iLedger.SoftDeleteLedgerAsync(ledger);
            if (operation_SoftDeleteLedger == null)
            {
                return NotFound("Unable to delete ledger information. Invalid ledger ID.");
            }
            return Ok("Ledger information deleted successfully.");
        }

        [HttpPut("undo-delete")]
        public async Task<IActionResult> UndoSoftDeleteLedger(Ledger ledger)
        {
            var operation_UndoSoftDeleteLedger = await _iLedger.UndoSoftDeleteLedgerAsync(ledger);
            if (operation_UndoSoftDeleteLedger == null)
            {
                return NotFound("Unable to restore deleted ledger information. Invalid ledger ID.");
            }
            return Ok("Ledger information restored successfully.");
        }
    }
}
