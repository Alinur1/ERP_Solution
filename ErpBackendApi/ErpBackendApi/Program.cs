using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.BLL.Services;
using ErpBackendApi.DAL.ERPDataContext;
using ErpBackendApi.Helper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using static ErpBackendApi.Helper.LoggerClass;

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

TODO
1. Add authentication in all controllers.
2. Add a role by default when a user registers.
3. Add a role by default when an admin creates an user.
4. Finish the role permission according to features.
5. Add proper validation to all the models.
6. Handle error messages properly.
7. Handle the date times properly to avoid extra works at the frontend.

*/