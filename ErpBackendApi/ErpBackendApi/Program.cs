using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.BLL.Services;
using ErpBackendApi.DAL.ERPDataContext;
using Microsoft.EntityFrameworkCore;
using static ErpBackendApi.Helper.LoggerClass;

Logger("\n===================Application Started===================");

var builder = WebApplication.CreateBuilder(args);

// Connection string configuration
var connectionString = builder.Configuration.GetConnectionString("ErpConnection");
builder.Services.AddDbContext<AppDataContext>(options => options.UseSqlServer(connectionString));

// Add services to the container.
builder.Services.AddScoped<IUsers, UserService>();

builder.Services.AddControllers();
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

app.UseAuthorization();

app.MapControllers();

app.Run();
