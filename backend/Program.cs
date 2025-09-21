using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using backend.Services;
using backend.Interfaces;
using backend.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ClickHouse.Client.ADO;
using System.Web;
using backend.Data; // Добавляем для Uri.EscapeDataString

var builder = WebApplication.CreateBuilder(args);

// Добавление сервисов
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5046",
                "https://vacya.netlify.app/"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .WithExposedHeaders("Authorization");
    });
});

builder.Services.AddScoped<IUser, UserService>();
builder.Services.AddScoped<WebsiteService>();
builder.Services.AddScoped<FilterService>();
builder.Services.AddScoped<TestScenarioService>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
    b => b.MigrationsAssembly("backend")    
));

builder.Services.AddControllers();

// Добавление аутентификации JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("super_secret_key_1234567890123456")),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

// Добавление авторизации
builder.Services.AddAuthorization();

builder.Services.AddHttpClient();

builder.Services.AddHostedService<MonitoringBackgroundService>();

var app = builder.Build();

// Настройка middleware pipeline
app.UseRouting();

// CORS должен быть до аутентификации и авторизации
app.UseCors("AllowFrontend");

// Аутентификация и авторизация
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();