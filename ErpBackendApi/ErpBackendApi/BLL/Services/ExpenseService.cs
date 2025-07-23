using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.DTOs;
using ErpBackendApi.DAL.ERPDataContext;
using ErpBackendApi.DAL.Models;
using Microsoft.EntityFrameworkCore;
using ZstdSharp.Unsafe;
using static ErpBackendApi.Utilities.Helper.LoggerClass;

namespace ErpBackendApi.BLL.Services
{
    public class ExpenseService : IExpenses
    {
        private readonly AppDataContext _context;
        public ExpenseService(AppDataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ExpenseDTO>> GetAllExpenseAsync()
        {
            return await
            (
                from e in _context.expenses
                join po in _context.purchase_orders on e.purchase_order_id equals po.id into purchaseGroup
                from po in purchaseGroup.DefaultIfEmpty()
                join p in _context.products on e.product_id equals p.id into productGroup
                from p in productGroup.DefaultIfEmpty()
                join c in _context.categories on p.category_id equals c.id into categoryGroup
                from c in categoryGroup.DefaultIfEmpty()
                where e.is_deleted == false
                select new ExpenseDTO
                {
                    id = e.id,
                    purchase_order_id = po != null ? po.id : null,
                    product_id = p != null ? p.id : null,
                    category_id = c != null ? c.id : null,
                    category_name = c != null && c.is_deleted == false ? c.name : "-",
                    description = e.description,
                    amount = e.amount,
                    expense_date = e.expense_date,
                }
            ).ToListAsync();
        }

        public async Task<ExpenseDTO> GetExpenseByIdAsync(int id)
        {
            return await
            (
                from e in _context.expenses
                join po in _context.purchase_orders on e.purchase_order_id equals po.id into purchaseGroup
                from po in purchaseGroup.DefaultIfEmpty()
                join p in _context.products on e.product_id equals p.id into productGroup
                from p in productGroup.DefaultIfEmpty()
                join c in _context.categories on p.category_id equals c.id into categoryGroup
                from c in categoryGroup.DefaultIfEmpty()
                where e.id == id && e.is_deleted == false
                select new ExpenseDTO
                {
                    id = e.id,
                    purchase_order_id = po != null ? po.id : null,
                    product_id = p != null ? p.id : null,
                    category_id = c != null ? c.id : null,
                    category_name = c != null && c.is_deleted == false ? c.name : "-",
                    description = e.description,
                    amount = e.amount,
                    expense_date = e.expense_date,
                }
            ).FirstOrDefaultAsync();
        }

        public async Task<Expense> AddExpenseAsync(Expense expense)
        {
            var existingExpense = await _context.expenses.FirstOrDefaultAsync(e => e.purchase_order_id == expense.purchase_order_id && e.is_deleted == false);
            if (existingExpense != null)
            {
                Logger("Unable to add expense. Same product order id already exists.");
                return null;
            }
            expense.is_deleted = false;
            expense.deleted_at = null;
            _context.expenses.Add(expense);
            await _context.SaveChangesAsync();
            return expense;
        }

        public async Task<Expense> UpdateExpenseAsync(Expense expense)
        {
            var existingExpense = await _context.expenses.FirstOrDefaultAsync(e => e.id == expense.id && e.is_deleted == false);
            if (existingExpense == null)
            {
                Logger("Unable to update expense.");
                return null;
            }
            existingExpense.product_id = expense.product_id;
            existingExpense.description = expense.description;
            existingExpense.amount = expense.amount;
            existingExpense.expense_date = expense.expense_date;

            _context.expenses.Update(existingExpense);
            await _context.SaveChangesAsync();
            return existingExpense;
        }

        public async Task<Expense> SoftDeleteExpenseAsync(Expense expense)
        {
            var existingExpense = await _context.expenses.FirstOrDefaultAsync(e => e.id == expense.id && e.is_deleted == false);
            if (existingExpense == null)
            {
                Logger("Unable to delete expense.");
                return null;
            }
            existingExpense.is_deleted = true;
            existingExpense.deleted_at = DateTime.UtcNow;

            _context.expenses.Update(existingExpense);
            await _context.SaveChangesAsync();
            return existingExpense;
        }

        public async Task<Expense> UndoSoftDeleteExpenseAsync(Expense expense)
        {
            var existingExpense = await _context.expenses.FirstOrDefaultAsync(e => e.id == expense.id && e.is_deleted == true);
            if (existingExpense == null)
            {
                Logger("Unable to restore deleted expense.");
                return null;
            }
            existingExpense.is_deleted = false;
            existingExpense.deleted_at = null;

            _context.expenses.Update(existingExpense);
            await _context.SaveChangesAsync();
            return existingExpense;
        }
    }
}
