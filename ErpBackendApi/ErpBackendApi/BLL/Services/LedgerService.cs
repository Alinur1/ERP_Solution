using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.DTOs;
using ErpBackendApi.DAL.ERPDataContext;
using ErpBackendApi.DAL.Models;

namespace ErpBackendApi.BLL.Services
{
    public class LedgerService : ILedger
    {
        private readonly AppDataContext _context;
        public LedgerService(AppDataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LedgerDTO>> GetAllLedgersAsync()
        {
            //return await
            //(
            //    from l in _context.ledgers
            //)
            throw new NotImplementedException();
        }

        public Task<LedgerDTO> GetLedgerByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Ledger> AddLedgerAsync(Ledger ledger)
        {
            throw new NotImplementedException();
        }

        public Task<Ledger> UpdateLedgerAsync(Ledger ledger)
        {
            throw new NotImplementedException();
        }

        public Task<Ledger> SoftDeleteLedgerAsync(Ledger ledger)
        {
            throw new NotImplementedException();
        }

        public Task<Ledger> UndoSoftDeleteLedgerAsync(Ledger ledger)
        {
            throw new NotImplementedException();
        }
    }
}
