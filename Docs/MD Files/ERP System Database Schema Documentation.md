# ERP System Database Schema Documentation

## 1. Users & Roles (Authentication + RBAC)

### Users
Stores user account information.
- **Id (PK)** - INT - Unique identifier
- **Name** - VARCHAR(255) - Full name of the user
- **Email** - VARCHAR(255) - Unique email address
- **Password** - VARCHAR(255) - Hashed password
- **Phone** - VARCHAR(50) - Contact phone number
- **CreatedAt** - DATETIME - Account creation timestamp

### Roles
Defines user roles.
- **Id (PK)** - INT
- **Name** - VARCHAR(100)
- **Description** - TEXT

### UserRoles
Mapping table for many-to-many relationship between users and roles.
- **Id (PK)** - INT
- **UserId (FK to Users.Id)** - INT
- **RoleId (FK to Roles.Id)** - INT

### Features
Defines individual access-controlled features.
- **Id (PK)** - INT
- **Name** - VARCHAR(100) (e.g. "Inventory.View")
- **Module** - VARCHAR(100) (e.g. "Inventory")
- **Description** - TEXT

### RolePermissions
Permissions associated with roles and features.
- **Id (PK)** - INT
- **RoleId (FK to Roles.Id)** - INT
- **FeatureId (FK to Features.Id)** - INT
- **CanRead** - BIT
- **CanCreate** - BIT
- **CanUpdate** - BIT
- **CanDelete** - BIT

## 2. Products & Inventory

### Categories
Defines product categories.
- **Id (PK)** - INT
- **Name** - VARCHAR(255)
- **Description** - TEXT

### Suppliers
Stores information about product suppliers.
- **Id (PK)** - INT
- **CompanyName** - VARCHAR(255)
- **ContactPersonName** - VARCHAR(255)
- **Phone** - VARCHAR(20)
- **Email** - VARCHAR(255)
- **Address** - TEXT

### Products
Product master table.
- **Id (PK)** - INT
- **Name** - VARCHAR(255)
- **CategoryId (FK to Categories.Id)** - INT
- **SupplierId (FK to Suppliers.Id)** - INT
- **SKU** - VARCHAR(100)
- **Description** - TEXT
- **Unit** - VARCHAR(50)
- **Price** - DECIMAL(12,2)
- **CreatedAt** - DATETIME

### Inventory
Tracks inventory levels per product.
- **Id (PK)** - INT
- **ProductId (FK to Products.Id)** - INT
- **Quantity** - INT
- **ReorderLevel** - INT (A threshold value — when Quantity drops below this, it flags a low stock alert)
- **LastUpdated** - DATETIME

## 3. Sales Module

### Customers
Customer master table.
- **Id (PK)** - INT
- **Name** - VARCHAR(255)
- **Email** - VARCHAR(255)
- **Phone** - VARCHAR(20)
- **Address** - TEXT

### SalesOrders
Tracks sales orders.
- **Id (PK)** - INT
- **OrderNumber** - VARCHAR(100)
- **CustomerId (FK to Customers.Id)** - INT
- **OrderDate** - DATE
- **DeliveryDate** - DATE
- **DeliveryStatus** - ENUM('Pending', 'Confirmed', 'Processing', 'Shipped', 'Delivered')
- **Status** - ENUM('Open', 'Closed', 'Cancelled')
- **Notes** - TEXT

### SalesOrderItems
Line items for sales orders.
- **Id (PK)** - INT
- **SalesOrderId (FK to SalesOrders.Id)** - INT
- **ProductId (FK to Products.Id)** - INT
- **Quantity** - INT
- **UnitPrice** - DECIMAL(12,2)
- **Discount** - DECIMAL(12,2)

### Invoices
Tracks invoices linked to sales orders.
- **Id (PK)** - INT
- **SalesOrderId (FK to SalesOrders.Id)** - INT
- **InvoiceDate** - DATE
- **TotalAmount** - DECIMAL(12,2)
- **IsPaid** - BIT
- **DueDate** - DATE

## 4. Purchase Module

### PurchaseOrders
Tracks purchase orders.
- **Id (PK)** - INT
- **OrderNumber** - VARCHAR(100)
- **SupplierId (FK to Suppliers.Id)** - INT
- **OrderDate** - DATE
- **ExpectedDeliveryDate** - DATE
- **DeliveryStatus** - VARCHAR(50)
- **Notes** - TEXT

### PurchaseOrderItems
Line items for purchase orders.
- **Id (PK)** - INT
- **PurchaseOrderId (FK to PurchaseOrders.Id)** - INT
- **ProductId (FK to Products.Id)** - INT
- **Quantity** - INT
- **UnitPrice** - DECIMAL(12,2)
- **Discount** - DECIMAL(12,2)

### Expenses
Tracks company expenses.
- **Id (PK)** - INT
- **PurchaseOrderId (FK to PurchaseOrders.Id, nullable)** - INT
- **Description** - TEXT
- **Amount** - DECIMAL(12,2)
- **ExpenseDate** - DATE
- **CategoryID** - INT (FK to an ExpenseCategories table if defined)

## 5. HR & Payroll

### Departments
Defines employee departments.
- **Id (PK)** - INT
- **Name** - VARCHAR(100)
- **Description** - TEXT

### Employees
Employee records.
- **Id (PK)** - INT
- **UserId (FK to Users.Id)** - INT
- **DepartmentId (FK to Departments.Id)** - INT
- **DateHired** - DATE
- **Salary** - DECIMAL(12,2)
- **Status** - ENUM('Active', 'On Leave', 'Terminated')

### Attendance
Tracks daily attendance.
- **Id (PK)** - INT
- **EmployeeId (FK to Employees.Id)** - INT
- **Date** - DATE
- **CheckIn** - TIME
- **CheckOut** - TIME
- **Status** - ENUM('Present', 'Absent', 'Leave')

### Payroll
Salary details per pay period.
- **Id (PK)** - INT
- **EmployeeId (FK to Employees.Id)** - INT
- **PeriodStart** - DATE
- **PeriodEnd** - DATE
- **BaseSalary** - DECIMAL(12,2)
- **Deductions** - DECIMAL(12,2)
- **Bonuses** - DECIMAL(12,2)
- **NetPay** - DECIMAL(12,2)
- **PaidOn** - DATE

## 6. Finance & Accounting

### Accounts
Chart of accounts.
- **Id (PK)** - INT
- **Name** - VARCHAR(255)
- **Type** - ENUM('Income', 'Expense', 'Asset', 'Liability', 'Equity')

### Transactions
Financial transactions per account.
- **Id (PK)** - INT
- **AccountId (FK to Accounts.Id)** - INT
- **Date** - DATE
- **Description** - TEXT
- **Amount** - DECIMAL(12,2)
- **Type** - ENUM('Credit', 'Debit')

### Ledgers
Double-entry accounting records.
- **Id (PK)** - INT
- **EntryDate** - DATE
- **Description** - TEXT
- **DebitAccountId (FK to Accounts.Id)** - INT
- **CreditAccountId (FK to Accounts.Id)** - INT
- **Amount** - DECIMAL(12,2)

## 7. Reports, Notifications, Logs

### Reports
User-generated or system-generated reports.
- **Id (PK)** - INT
- **Name** - VARCHAR(255)
- **Module** - VARCHAR(100)
- **CreatedBy (FK to Users.Id)** - INT
- **CreatedAt** - DATETIME
- **FiltersJson** - TEXT (storing filter criteria)

### Notifications
User-targeted alerts.
- **Id (PK)** - INT
- **UserId (FK to Users.Id)** - INT
- **Title** - VARCHAR(255)
- **Message** - TEXT
- **IsRead** - BIT
- **CreatedAt** - DATETIME

## 8. System Settings & Company Profile

### Settings
Key-value settings for global configuration.
- **Id (PK)** - INT
- **Key** - VARCHAR(100)
- **Value** - TEXT
- **UpdatedAt** - DATETIME

### CompanyProfile
Basic company info.
- **Id (PK)** - INT
- **CompanyName** - VARCHAR(255)
- **Address** - TEXT
- **Email** - VARCHAR(255)
- **Phone** - VARCHAR(20)
- **TaxNumber** - VARCHAR(100)
- **Logo** - TEXT (URL or file path)

---

**Note:** All foreign key constraints should be enforced for data integrity. Timestamps such as CreatedAt, UpdatedAt, and DeletedAt can be added as needed to support audit logging and soft deletes.