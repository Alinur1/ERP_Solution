# ERP System Database Schema Documentation

## Schema/Database name = erp_api

## A. Users & Roles (Authentication + RBAC)

### 1. users
Stores user account information.
- **id** (PK) - INT - Unique identifier
- **name** - VARCHAR(255) - Full name of the user
- **email** - VARCHAR(255) - Unique email address
- **password** - VARCHAR(255) - Hashed password
- **phone** - VARCHAR(50) - Contact phone number
- **created_at** - DATETIME - Account creation timestamp
- **is_deleted** - BIT
- **deleted_at** - DATETIME

### 2. roles
Defines user roles.
- **id** (PK) - INT
- **name** - VARCHAR(100)
- **description** - TEXT
- **is_deleted** - BIT
- **deleted_at** - DATETIME

### 3. user_roles
Mapping table for many-to-many relationship between users and roles.
- **id** (PK) - INT
- **user_id** (FK to users.id) - INT
- **role_id** (FK to roles.id) - INT

### 4. features
Defines individual access-controlled features.
- **id** (PK) - INT [No auto increment]
- **name** - VARCHAR(100) (e.g. "Inventory.View")
- **module** - VARCHAR(100) (e.g. "Inventory")
- **description** - TEXT

### 5. role_permissions
Permissions associated with roles and features.
- **id** (PK) - INT
- **role_id** (FK to roles.id) - INT
- **feature_id** (FK to features.id) - INT
- **can_read** - BIT
- **can_create** - BIT
- **can_update** - BIT
- **can_delete** - BIT

## B. Products & Inventory

### 6. categories
Defines product categories.
- **id** (PK) - INT
- **name** - VARCHAR(255)
- **description** - TEXT
- **is_deleted** - BIT
- **deleted_at** - DATETIME

### 7. suppliers
Stores information about product suppliers.
- **id** (PK) - INT
- **company_name** - VARCHAR(255)
- **contact_person_name** - VARCHAR(255)
- **phone** - VARCHAR(50)
- **email** - VARCHAR(255)
- **address** - TEXT
- **is_deleted** - BIT
- **deleted_at** - DATETIME

### 8. products
Product master table.
- **id** (PK) - INT
- **name** - VARCHAR(255)
- **category_id** (FK to categories.id) - INT
- **supplier_id** (FK to suppliers.id) - INT
- **sku** - VARCHAR(100)
- **description** - TEXT
- **unit** - VARCHAR(50)
- **price** - DECIMAL(12,2)
- **created_at** - DATETIME
- **is_deleted** - BIT
- **deleted_at** - DATETIME

### 9. inventory
Tracks inventory levels per product.
- **id** (PK) - INT
- **product_id** (FK to products.id) - INT
- **quantity** - INT
- **reorder_level** - INT (A threshold value — when Quantity drops below this, it flags a low stock alert)
- **last_updated** - DATETIME

## C. Sales Module

### 10. customers
Customer master table.
- **id** (PK) - INT
- **name** - VARCHAR(255)
- **email** - VARCHAR(255)
- **phone** - VARCHAR(50)
- **address** - TEXT
- **is_deleted** - BIT
- **deleted_at** - DATETIME

### 11. sales_orders
Tracks sales orders.
- **id** (PK) - INT
- **order_number** - VARCHAR(100)
- **customer_id** (FK to customers.id) - INT
- **order_date** - DATE
- **delivery_date** - DATE
- **delivery_status** - ENUM - INT - ('Pending', 'Confirmed', 'Processing', 'Shipped', 'Delivered')
- **status** - ENUM - INT - ('Open', 'Closed', 'Cancelled')
- **notes** - TEXT
- **is_deleted** - BIT
- **deleted_at** - DATETIME

### 12. sales_order_items
Line items for sales orders.
- **id** (PK) - INT
- **sales_order_id** (FK to sales_orders.id) - INT
- **product_id** (FK to products.id) - INT
- **quantity** - INT
- **unit_price** - DECIMAL(12,2)
- **discount** - DECIMAL(12,2)
- **is_deleted** - BIT
- **deleted_at** - DATETIME

### 13. invoices
Tracks invoices linked to sales orders.
- **id** (PK) - INT
- **sales_order_id** (FK to sales_orders.id) - INT
- **invoice_date** - DATE
- **total_amount** - DECIMAL(12,2)
- **is_paid** - BIT
- **due_date** - DATE
- **is_deleted** - BIT
- **deleted_at** - DATETIME

## D. Purchase Module

### 14. purchase_orders
Tracks purchase orders.
- **id** (PK) - INT
- **order_number** - VARCHAR(100)
- **supplier_id** (FK to suppliers.id) - INT
- **order_date** - DATE
- **expected_delivery_date** - DATE
- **delivery_status** - INT [Pending, Confirmed, InTransit, PartiallyDelivered, Delivered, Cancelled]
- **notes** - TEXT
- **is_deleted** - BIT
- **deleted_at** - DATETIME

### 15. purchase_order_items
Line items for purchase orders.
- **id** (PK) - INT
- **purchase_order_id** (FK to purchase_orders.id) - INT
- **product_id** (FK to products.id) - INT
- **quantity** - INT
- **unit_price** - DECIMAL(12,2)
- **discount** - DECIMAL(12,2)
- **is_deleted** - BIT
- **deleted_at** - DATETIME

### 16. expenses
Tracks company expenses.
- **id** (PK) - INT
- **purchase_order_id** (FK to purchase_orders.id, nullable) - INT
- **description** - TEXT
- **amount** - DECIMAL(12,2)
- **expense_date** - DATE
- **category_id** - INT (FK to an expense_categories table if defined)
- **is_deleted** - BIT
- **deleted_at** - DATETIME

## E. HR & Payroll

### 17. departments
Defines employee departments.
- **id** (PK) - INT
- **name** - VARCHAR(100)
- **description** - TEXT
- **is_deleted** - BIT
- **deleted_at** - DATETIME

### 18. employees
Employee records.
- **id** (PK) - INT
- **user_id** (FK to users.id) - INT
- **department_id** (FK to departments.id) - INT
- **date_hired** - DATE
- **salary** - DECIMAL(12,2)
- **status** - ENUM('Active', 'On Leave', 'Terminated')
- **is_deleted** - BIT
- **deleted_at** - DATETIME

### 19. attendance
Tracks daily attendance.
- **id** (PK) - INT
- **employee_id** (FK to employees.id) - INT
- **date_of_attendance** - DATE
- **check_in** - TIME
- **check_out** - TIME
- **status** - ENUM('Present', 'Absent', 'Leave')

### 20. payroll
Salary details per pay period.
- **id** (PK) - INT
- **employee_id** (FK to employees.id) - INT
- **period_start** - DATE
- **period_end** - DATE
- **base_salary** - DECIMAL(12,2)
- **deductions** - DECIMAL(12,2)
- **bonuses** - DECIMAL(12,2)
- **net_pay** - DECIMAL(12,2)
- **paid_on** - DATE
- **is_deleted** - BIT
- **deleted_at** - DATETIME

## F. Finance & Accounting

### 21. accounts
Chart of accounts.
- **id** (PK) - INT
- **name** - VARCHAR(255)
- **type** - ENUM('Income', 'Expense', 'Asset', 'Liability', 'Equity')
- **is_deleted** - BIT
- **deleted_at** - DATETIME

### 22. transactions
Financial transactions per account.
- **id** (PK) - INT
- **account_id** (FK to accounts.id) - INT
- **transaction_date** - DATE
- **description** - TEXT
- **amount** - DECIMAL(12,2)
- **type** - ENUM('Credit', 'Debit')
- **is_deleted** - BIT
- **deleted_at** - DATETIME

### 23. ledgers
Double-entry accounting records.
- **id** (PK) - INT
- **entry_date** - DATE
- **description** - TEXT
- **debit_account_id** (FK to accounts.id) - INT
- **credit_account_id** (FK to accounts.id) - INT
- **amount** - DECIMAL(12,2)
- **is_deleted** - BIT
- **deleted_at** - DATETIME

## G. Reports, Notifications, Logs

### 24. reports
User-generated or system-generated reports.
- **id** (PK) - INT
- **name** - VARCHAR(255)
- **module** - VARCHAR(100)
- **created_by** (FK to users.id) - INT
- **created_at** - DATETIME
- **filters_json** - TEXT (storing filter criteria)

### 25. notifications
User-targeted alerts.
- **id** (PK) - INT
- **user_id** (FK to users.id) - INT
- **title** - VARCHAR(255)
- **message** - TEXT
- **is_read** - BIT
- **created_at** - DATETIME

## H. System Settings & Company Profile

### 26. settings
Key-value settings for global configuration.
- **id** (PK) - INT
- **key** - VARCHAR(100)
- **value** - TEXT
- **updated_at** - DATETIME

### 27. company_profile
Basic company info.
- **id** (PK) - INT
- **company_name** - VARCHAR(255)
- **address** - TEXT
- **email** - VARCHAR(255)
- **phone** - VARCHAR(50)
- **tax_number** - VARCHAR(100)
- **logo** - TEXT (URL or file path)
- **is_deleted** - BIT
- **deleted_at** - DATETIME

---

**Note:** All foreign key constraints should be enforced for data integrity. Timestamps such as `created_at`, `updated_at`, and `deleted_at` can be added as needed to support audit logging and soft deletes.