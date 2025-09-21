using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.BLL.TransactionInterface;
using ErpBackendApi.DAL.DTOs;
using ErpBackendApi.DAL.ERPDataContext;
using ErpBackendApi.DAL.Models;
using Microsoft.EntityFrameworkCore;
using static ErpBackendApi.Utilities.Helper.LoggerClass;

namespace ErpBackendApi.BLL.Services
{
    public class ExpenseService : IExpenses
    {
        private readonly AppDataContext _context;
        private readonly ITransactionExpenseGenerator _transactionGenerator;
        public ExpenseService(AppDataContext context, ITransactionExpenseGenerator transactionGenerator)
        {
            _context = context;
            _transactionGenerator = transactionGenerator;
        }

        public async Task<IEnumerable<ExpenseDTO>> GetAllExpenseAsync()
        {
            var expenses = await _context.expenses
                .Where(e => e.is_deleted == false)
                .Select(e => new ExpenseDTO
                {
                    id = e.id,
                    description = e.description,
                    total_amount = e.total_amount,
                    expense_date = e.expense_date,
                    is_deleted = e.is_deleted,
                    deleted_at = e.deleted_at,

                    purchase_order = _context.purchase_orders
                        .Where(po => po.id == e.purchase_order_id && po.is_deleted == false)
                        .Select(po => new PurchaseOrderDTO
                        {
                            id = po.id,
                            supplier_id = po.supplier_id,
                            order_date = po.order_date,
                            expected_delivery_date = po.expected_delivery_date,
                            delivery_status = po.delivery_status,
                            notes = po.notes,
                            is_deleted = po.is_deleted,
                            deleted_at = po.deleted_at,

                            supplier = _context.suppliers
                                .Where(s => s.id == po.supplier_id && s.is_deleted == false)
                                .Select(s => new SupplierDTO
                                {
                                    id = s.id,
                                    company_name = s.company_name,
                                    contact_person_name = s.contact_person_name,
                                    phone = s.phone,
                                    email = s.email,
                                    address = s.address
                                })
                                .FirstOrDefault(),

                            items = _context.purchase_order_items
                                .Where(poi => poi.purchase_order_id == po.id && poi.is_deleted == false)
                                .Select(poi => new PurchaseOrderItemDTO
                                {
                                    id = poi.id,
                                    product_id = poi.product_id,
                                    quantity = poi.quantity,
                                    amount = poi.amount,
                                    discount = poi.discount,
                                    product = _context.products
                                        .Where(p => p.id == poi.product_id && p.is_deleted == false)
                                        .Select(p => new ProductDTO
                                        {
                                            id = p.id,
                                            name = p.name,
                                            sku = p.sku,
                                            unit = p.unit,
                                            price = p.price
                                        })
                                        .FirstOrDefault()
                                })
                                .ToList()
                        })
                        .FirstOrDefault()
                })
                .ToListAsync();

            return expenses;
        }

        public async Task<ExpenseDTO> GetExpenseByIdAsync(int id)
        {
            return await GetAllExpenseAsync().ContinueWith(t => t.Result.FirstOrDefault(e => e.id == id));
        }

        public async Task<Expense> AddExpenseAsync(Expense expense)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existingExpense = await _context.expenses.FirstOrDefaultAsync(e => e.purchase_order_id == expense.purchase_order_id && e.is_deleted == false);
                if (existingExpense != null)
                {
                    Logger("Unable to add expense. Same purchase order id already exists.");
                    throw new InvalidOperationException("Unable to add expense. Same purchase order id already exists.");
                }

                var purchaseOrderItems = await _context.purchase_order_items
                    .Where(poi => poi.purchase_order_id == expense.purchase_order_id && poi.is_deleted == false)
                    .ToListAsync();
                if (purchaseOrderItems == null || purchaseOrderItems.Count == 0)
                {
                    Logger("Cannot create expense without purchase order items.");
                    throw new InvalidOperationException("Cannot create expense without purchase order items.");
                }

                // ADD to inventory (opposite of invoice which deducts)
                foreach (var item in purchaseOrderItems)
                {
                    var inventory = await _context.inventory.FirstOrDefaultAsync(inv => inv.product_id == item.product_id && inv.is_deleted == false);
                    if (inventory == null)
                    {
                        // Create new inventory entry if it doesn't exist
                        inventory = new Inventory
                        {
                            product_id = item.product_id,
                            quantity = item.quantity,
                            reorder_level = 10, // Default reorder level
                            last_updated = DateTime.UtcNow,
                            is_deleted = false
                        };
                        _context.inventory.Add(inventory);
                    }
                    else
                    {
                        // Add to existing inventory
                        inventory.quantity += item.quantity;
                        inventory.last_updated = DateTime.UtcNow;
                    }
                }

                // Calculate expense total (similar to invoice but for purchases)
                expense.total_amount = purchaseOrderItems.Sum(item =>
                {
                    var discount = item.discount ?? 0;
                    var lineTotal = item.amount ?? 0;
                    return discount <= 1
                        ? lineTotal - (lineTotal * discount)   // treat <=1 as percentage
                        : lineTotal - discount;                // treat >1 as flat amount
                });

                expense.is_deleted = false;
                expense.deleted_at = null;
                _context.expenses.Add(expense);
                await _context.SaveChangesAsync();

                // GENERATE AUTOMATIC TRANSACTIONS!
                string desc = $"Purchase Order #{expense.purchase_order_id}";
                await _transactionGenerator.GenerateExpenseTransactionsAsync(expense, desc);

                await transaction.CommitAsync();
                return expense;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await transaction.RollbackAsync();
                Logger($"Concurrency conflict in AddExpenseAsync: {ex.Message}");
                throw new InvalidOperationException("Inventory was modified by another transaction. Please try again.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Logger($"Error in AddExpenseAsync: {ex.Message}");
                throw;
            }
        }
    }
}
