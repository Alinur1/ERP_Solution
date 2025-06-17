using ErpBackendApi.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace ErpBackendApi.DAL.ERPDataContext
{
    public class AppDataContext : DbContext
    {
        public AppDataContext(DbContextOptions<AppDataContext> options) : base(options)
        {

        }

        //Users & Roles (Authentication + RBAC)
        public DbSet<User> users { get; set; }
        public DbSet<Role> roles { get; set; }
        public DbSet<UserRole> user_role { get; set; }
        public DbSet<Feature> features { get; set; }
        public DbSet<RolePermission> role_permissions { get; set; }

        //Products & Inventory
        public DbSet<Category> categories { get; set; }
        public DbSet<Supplier> suppliers { get; set; }
        public DbSet<Product> products { get; set; }
        public DbSet<Inventory> inventory { get; set; }
        public DbSet<Customer> customers { get; set; }
        public DbSet<SalesOrder> sales_orders { get; set; }
        public DbSet<SalesOrderItem> sales_order_items { get; set; }
        public DbSet<Invoice> invoices { get; set; }

        //Purchase Module
        public DbSet<PurchaseOrder> purchase_orders { get; set; }
        public DbSet<PurchaseOrderItem> purchase_order_items { get; set; }
        public DbSet<Expense> expenses { get; set; }

        //HR & Payroll
        public DbSet<Department> departments { get; set; }
        public DbSet<Employee> employees { get; set; }
        public DbSet<Attendance> attendance { get; set; }
        public DbSet<Payroll> payroll { get; set; }

        //Finance & Accounting
        public DbSet<Account> accounts { get; set; }
        public DbSet<Transaction> transactions { get; set; }
        public DbSet<Ledger> ledgers { get; set; }

        //Reports, Notifications, Logs
        public DbSet<Report> reports { get; set; }
        public DbSet<Notification> notifications { get; set; }

        //System Settings & Company Profile
        public DbSet<Setting> settings { get; set; }
        public DbSet<CompanyProfile> company_profile { get; set; }
    }
}