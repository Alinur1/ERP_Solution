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
            ValidIssuer = jwtConfig["Issuer"],
            ValidAudience = jwtConfig["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["Key"]))
        };
    });


// Add services to the container.
builder.Services.AddScoped<IUsers, UserService>();
builder.Services.AddScoped<IRoles, RoleService>();
builder.Services.AddScoped<IUserRoles, UserRoleService>();
builder.Services.AddScoped<IRolePermissions, RolePermissionService>();
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



---------------------------------------------------------------------------------------------------------------------------------
Testing TODO
---------------------------------------------------------------------------------------------------------------------------------
1. = COMPLETE = Test Inventory API
2. = COMPLETE = Test Customer API
3. Test SalesOrder API
4. Test SalesOrderItem API
5. Test Invoice API
6. Test PurchaseOrder API
7. Test PurchaseOrderItem API
8. Test Expense API

*/