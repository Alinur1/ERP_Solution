using ErpBackendApi.BLL.TransactionInterface;
using ErpBackendApi.DAL.Enums;
using ErpBackendApi.DAL.ERPDataContext;
using ErpBackendApi.DAL.Models;
using Microsoft.EntityFrameworkCore;
using static ErpBackendApi.Utilities.Helper.LoggerClass;

namespace ErpBackendApi.BLL.TransactionService
{
    public class TransactionPayrollGenerator : ITransactionPayrollGenerator
    {
        private readonly AppDataContext _context;

        public TransactionPayrollGenerator(AppDataContext context)
        {
            _context = context;
        }

        // Account ID constants for PAYROLL-RELATED accounts
        private const int SalariesWagesExpenseAccountId = 8;  // Expense (Debit)
        private const int SalariesPayableAccountId = 23;      // Liability (Credit)

        public async Task GeneratePayrollTransactionsAsync(Payroll payroll, string description)
        {
            var salariesExpenseAccount = await _context.accounts.FirstOrDefaultAsync(a => a.id == SalariesWagesExpenseAccountId && a.is_deleted == false);
            var salariesPayableAccount = await _context.accounts.FirstOrDefaultAsync(a => a.id == SalariesPayableAccountId && a.is_deleted == false);

            if (salariesExpenseAccount == null || salariesPayableAccount == null)
            {
                Logger("Critical system accounts (Salaries & Wages Expense or Salaries Payable) not found for payroll transaction.");
                throw new InvalidOperationException("Critical system accounts not found. Cannot generate payroll transactions.");
            }

            //Calculate Gross Pay (Net Pay + Deductions - Bonuses)
            //Note: This is a simplification. A real system would have a defined Gross Pay.
            decimal grossPayAmount = (payroll.net_pay ?? 0) + (payroll.deductions ?? 0) - (payroll.bonuses ?? 0);

            var transactions = new List<Transaction>();

            // Debit the Expense account for the Gross Pay
            transactions.Add(new Transaction
            {
                account_id = SalariesWagesExpenseAccountId,
                transaction_date = payroll.paid_on ?? DateTime.UtcNow, // Use payment date, or now
                description = $"PAYROLL-{payroll.id}: {description} (Gross Wage)",
                amount = grossPayAmount,
                normal_balance = DebitCreditType.Debit, // Expenses increase with Debit
                is_deleted = false,
                deleted_at = null
            });

            // If there are Bonuses, they are also an expense (part of Gross Pay)
            // The debit above already includes them. We don't need a separate transaction.

            // Credit the Liability account for the Net Pay (what the employee receives)
            transactions.Add(new Transaction
            {
                account_id = SalariesPayableAccountId,
                transaction_date = payroll.paid_on ?? DateTime.UtcNow,
                description = $"PAYROLL-{payroll.id}: {description} (Net Pay)",
                amount = payroll.net_pay,
                normal_balance = DebitCreditType.Credit, // Liabilities increase with Credit
                is_deleted = false,
                deleted_at = null
            });

            //Save Transactions
            await _context.transactions.AddRangeAsync(transactions);
            await _context.SaveChangesAsync();
        }
    }
}
