using FinanceProject.Application.Models;
using FinanceProject.BusinessLayer.Abstract;
using FinanceProject.BusinessLayer.Concreate;
using FinanceProject.BusinessLayer.Concreate.Jwt;
using FinanceProject.DataAccesLayer.Abstract;
using FinanceProject.DataAccesLayer.Concreate;
using FinanceProject.DataAccesLayer.Dapper;
using FinanceProject.WebApi.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
var jwtKey = builder.Configuration["JwtSettings:SecretKey"];
var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddHttpContextAccessor();

// IHttpContextAccessor'ý kullanabilmek için ek bir hizmet tanýmlayýn


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

    // Cookie'den JWT token almak için gerekli ayarlar
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
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("http://localhost:3000") // Buraya kendi frontend orijininizi ekleyin
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials(); // Bu satýrý ekleyin
        });
});


builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.IgnoreNullValues = true; // Örnek bir ayar
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase; // Örnek bir ayar
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUserService, UserManager>();
builder.Services.AddScoped<IAccountService, AccountManager>();
builder.Services.AddScoped<ITransactionHistoryService, TransactionHistoryManager>();
builder.Services.AddScoped<IVerificationCodeService, VerificationCodeManager>();
builder.Services.AddScoped<IEmailService, EmailManager>();

builder.Services.AddScoped(typeof(BaseResponse));


builder.Services.AddScoped<JwtService>();


// Add your IDbConnection dependency
builder.Services.AddScoped<IDbConnection>(provider => new ConnectionHelper(provider.GetRequiredService<IConfiguration>()).CreateConnection());

// Register your DAL implementations
builder.Services.AddScoped<IUserDal, DpUserDal>();
builder.Services.AddScoped<IAccountDal, DpAccountDal>();
builder.Services.AddScoped<ITransactionHistoryDal, DpTransactionHistoryDal>();
builder.Services.AddScoped<IVerificationCodeDal, DpVerificationCodeDal>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseCors("AllowSpecificOrigin"); // Burada oluþturduðunuz policy adýný kullanýn
app.UseAuthorization();
app.UseCors();

app.MapControllers();

app.Run();
