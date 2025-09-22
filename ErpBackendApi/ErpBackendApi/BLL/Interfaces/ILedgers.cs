using ErpBackendApi.DAL.DTOs;
using ErpBackendApi.DAL.Models;

namespace ErpBackendApi.BLL.Interfaces
{
    public interface ILedgers
    {
        Task<IEnumerable<LedgerDTO>> GetAllLedgersAsync();
        Task<LedgerDTO> GetLedgerByIdAsync(int id);
        Task<Ledger> AddLedgerAsync(Ledger ledger);
    }
}
