using FinanceProject.BusinessLayer.Abstract;
using FinanceProject.BusinessLayer.Concreate;
using FinanceProject.DataAccesLayer.Abstract;
using FinanceProject.DataAccesLayer.Concreate;
using FinanceProject.DataAccesLayer.Dapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IUserService, UserManager>();
builder.Services.AddScoped<IAccountService, AccountManager>();
builder.Services.AddScoped<ITransactionHistoryService, TransactionHistoryManager>();

// Add your IDbConnection dependency
builder.Services.AddScoped<IDbConnection>(provider => new ConnectionHelper(provider.GetRequiredService<IConfiguration>()).CreateConnection());

// Register your DAL implementations
builder.Services.AddScoped<IUserDal, DpUserDal>();
builder.Services.AddScoped<IAccountDal, DpAccountDal>();
builder.Services.AddScoped<ITransactionHistoryDal, DpTransactionHistoryDal>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
