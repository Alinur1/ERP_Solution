using ErpBackendApi.DAL.DTOs;
using ErpBackendApi.DAL.Models;

namespace ErpBackendApi.BLL.Interfaces
{
    public interface ILedger
    {
        Task<IEnumerable<LedgerDTO>> GetAllLedgersAsync();
        Task<LedgerDTO> GetLedgerByIdAsync(int id);
        Task<Ledger> AddLedgerAsync(Ledger ledger);
        Task<Ledger> UpdateLedgerAsync(Ledger ledger);
        Task<Ledger> SoftDeleteLedgerAsync(Ledger ledger);
        Task<Ledger> UndoSoftDeleteLedgerAsync(Ledger ledger);
    }
}
