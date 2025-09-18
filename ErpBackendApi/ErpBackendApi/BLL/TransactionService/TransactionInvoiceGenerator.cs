using ErpBackendApi.BLL.TransactionInterface;
using ErpBackendApi.DAL.Enums;
using ErpBackendApi.DAL.ERPDataContext;
using ErpBackendApi.DAL.Models;
using Microsoft.EntityFrameworkCore;
using static ErpBackendApi.Utilities.Helper.LoggerClass;

namespace ErpBackendApi.BLL.TransactionService
{
    public class TransactionInvoiceGenerator : ITransactionInvoiceGenerator
    {
        private readonly AppDataContext _context;

        public TransactionInvoiceGenerator(AppDataContext context)
        {
            _context = context;
        }

        // Account ID constants for INVOICE-RELATED accounts only
        private const int SalesRevenueAccountId = 1;          // Income
        private const int AccountsReceivableAccountId = 15;   // Asset

        public async Task GenerateInvoiceTransactionsAsync(Invoice invoice, string description)
        {
            try
            {
                var salesRevenueAccount = await _context.accounts.FirstOrDefaultAsync(a => a.id == SalesRevenueAccountId && a.is_system_account == true);
                var accountsReceivableAccount = await _context.accounts.FirstOrDefaultAsync(a => a.id == AccountsReceivableAccountId && a.is_system_account == true);

                if (salesRevenueAccount == null || accountsReceivableAccount == null)
                {
                    Logger("Required system accounts not found for invoice transactions.");
                    throw new InvalidOperationException("Accounting system configuration error. Please contact administrator.");
                }

                var transactions = new List<Transaction>
                {
                    CreateTransaction(AccountsReceivableAccountId, invoice.total_amount, DebitCreditType.Debit, $"INV-{invoice.id}: {description}", invoice.invoice_date),
                    CreateTransaction(SalesRevenueAccountId, invoice.total_amount, DebitCreditType.Credit, $"INV-{invoice.id}: {description}", invoice.invoice_date)
                };

                await _context.transactions.AddRangeAsync(transactions);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger($"Error generating invoice transactions: {ex.Message}");
                throw new InvalidOperationException("Failed to generate accounting entries for invoice.");
            }
        }

        public async Task ReverseInvoiceTransactionsAsync(int invoiceId, string reason)
        {
            try
            {
                var transactions = await _context.transactions
                    .Where(t => t.description.Contains($"INV-{invoiceId}:") && t.is_deleted == false)
                    .ToListAsync();

                if (transactions.Count == 0) return;

                foreach (var transactionEntry in transactions)
                {
                    transactionEntry.is_deleted = true;
                    transactionEntry.deleted_at = DateTime.UtcNow;
                    transactionEntry.description += $" - Reversed: {reason}";
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger($"Error reversing invoice transactions: {ex.Message}");
                throw new InvalidOperationException("Failed to reverse accounting entries for invoice.");
            }
        }

        private Transaction CreateTransaction(int accountId, decimal? amount, DebitCreditType normalBalance, string description, DateTime? transactionDate)
        {
            return new Transaction
            {
                account_id = accountId,
                amount = amount,
                normal_balance = normalBalance,
                description = description,
                transaction_date = transactionDate ?? DateTime.UtcNow,
                is_deleted = false,
                deleted_at = null
            };
        }
    }
}
