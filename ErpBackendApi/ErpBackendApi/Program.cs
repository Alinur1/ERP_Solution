using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.BLL.Services;
using ErpBackendApi.DAL.ERPDataContext;
using ErpBackendApi.Utilities.Helper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using static ErpBackendApi.Utilities.Helper.LoggerClass;

Logger("\n===================Application Started===================");

var builder = WebApplication.CreateBuilder(args);

// Connection string configuration
var connectionString = builder.Configuration.GetConnectionString("ErpConnection");
builder.Services.AddDbContext<AppDataContext>(options => options.UseMySQL(connectionString));  //For MySQL, uncomment this line and comment the SQL Server line below.
//builder.Services.AddDbContext<AppDataContext>(options => options.UseSqlServer(connectionString)); //For SQL Server, uncomment this line and comment the MySQL line above.


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtConfig = builder.Configuration.GetSection("Jwt");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = jwtConfig["Issuer"],
            ValidAudience = jwtConfig["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["Key"]))
        };
    });


// Add services to the container.
builder.Services.AddScoped<IUsers, UserService>();
builder.Services.AddScoped<ICategories, CategoryService>();
builder.Services.AddScoped<ISuppliers, SupplierService>();
builder.Services.AddScoped<IProducts, ProductService>();
builder.Services.AddScoped<IInventories, InventoryService>();
builder.Services.AddScoped<ICustomers, CustomerService>();
builder.Services.AddScoped<ISalesOrders, SalesOrderService>();
builder.Services.AddScoped<ISalesOrderItems, SalesOrderItemService>();
builder.Services.AddScoped<IInvoices, InvoiceService>();
builder.Services.AddScoped<IPurchaseOrders, PurchaseOrderService>();
builder.Services.AddScoped<IPurchaseOrderItems, PurchaseOrderItemService>();
builder.Services.AddScoped<IExpenses, ExpenseService>();
builder.Services.AddScoped<IDepartments, DepartmentService>();
builder.Services.AddScoped<IEmployees, EmployeeService>();
builder.Services.AddScoped<IAttendances, AttendanceService>();
builder.Services.AddScoped<IPayrolls, PayrollService>();
builder.Services.AddScoped<IAccounts, AccountService>();
builder.Services.AddScoped<ITransactions, TransactionService>();
builder.Services.AddScoped<ILedgers, LedgerService>();
builder.Services.AddScoped<IReports, ReportService>();
builder.Services.AddScoped<INotifications, NotificationService>();
builder.Services.AddScoped<ISettings, SettingService>();
builder.Services.AddScoped<ICompanyProfile, CompanyProfileService>();


builder.Services.AddSingleton<JwtHelper>();
builder.Services.AddAuthorization();
builder.Services.AddControllers();



// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // React App
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigins");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();



/* 

---------------------------------------------------------------------------------------------------------------------------------
TODO
---------------------------------------------------------------------------------------------------------------------------------
1. Add authentication in all controllers.
2. Add a role by default when a user registers.
3. Add a role by default when an admin creates an user.
4. Finish the role permission according to features.
5. Add proper validation to all the models.
6. Handle error messages properly.
7. Handle the date times properly to avoid extra works at the frontend.
8. KillSwitch for feature permission.[ if bool FeaturePermission == true, admin can use role_permission feature, else, only admin role is accessible. ]
9. = REPEAT =  = COMPLETE = Check how to show the values which are depended on foreign keys and their refereneced id has a value of "is_delete = true".
10. Add unit testing project.
11. Handle null/no values (specially for string) variables gracefully for frontend.
12. Automatically generate Order number for sales_order.
13. In AppDataContext, try to convert all the objects of the models to the plural form.
14. Change all the DateOnly to DateTime
15. Add pagination as necessity. Check the PaginatedResult.cs in the Utilites/Helper folder.
16. Add transaction [CommitAsync(), RollbackAsync()] in services as necessary (except the methods that uses HTTPGET).



---------------------------------------------------------------------------------------------------------------------------------
Testing TODO
---------------------------------------------------------------------------------------------------------------------------------
1. "ready to implement frontend" - users
2. "will be implemented in future" - roles
3. "will be implemented in future" - user_roles
4. "will be implemented in future" - features
5. "will be implemented in future" - role_permissions
6. "ready to implement frontend" - categories
7. "ready to implement frontend" - suppliers
8. "ready to implement frontend" - products
9. "ready to implement frontend" - inventory
10. "ready to implement frontend" - customers
11. "ready to implement frontend" - sales_orders
12. "ready to implement frontend" - sales_order_items
13. "ready to implement frontend" - invoices
14. purchase_orders
15. purchase_order_items
16. expenses
17. departments
18. employees
19. attendance
20. payroll
21. accounts
22. transactions
23. ledgers
24. reports
25. notifications
26. settings
27. company_profile

*/
