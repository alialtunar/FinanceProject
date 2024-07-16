using FinanceProject.BusinessLayer.Abstract;
using FinanceProject.BusinessLayer.Concreate;
using FinanceProject.BusinessLayer.Concreate.Jwt;
using FinanceProject.DataAccesLayer.Abstract;
using FinanceProject.DataAccesLayer.Concreate;
using FinanceProject.DataAccesLayer.Dapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
var jwtKey = builder.Configuration["JwtSettings:SecretKey"];
var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddHttpContextAccessor();

// IHttpContextAccessor'� kullanabilmek i�in ek bir hizmet tan�mlay�n


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "JwtBearer";
    options.DefaultChallengeScheme = "JwtBearer";
}).AddJwtBearer("JwtBearer", jwtBearerOptions =>
{
    jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

    // Cookie'den JWT token almak i�in gerekli ayarlar
    jwtBearerOptions.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["JWTToken"];
            return Task.CompletedTask;
        }
    };
});


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUserService, UserManager>();
builder.Services.AddScoped<IAccountService, AccountManager>();
builder.Services.AddScoped<ITransactionHistoryService, TransactionHistoryManager>();
builder.Services.AddScoped<JwtService>();


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

app.UseAuthentication();
app.UseAuthorization();
app.UseCors();

app.MapControllers();

app.Run();
