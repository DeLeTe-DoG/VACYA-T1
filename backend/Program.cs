using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using backend.Services;
using backend.Interfaces;
using backend.Models;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

// Добавление сервисов
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<IUser>(sp => sp.GetRequiredService<UserService>());
builder.Services.AddSingleton(new List<WebSiteDTO>());
builder.Services.AddSingleton<WebsiteService>();
builder.Services.AddSingleton<FilterService>();
builder.Services.AddSingleton<TestScenarioService>();


builder.Services.AddControllers();

// Добавление аутентификации JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("super_secret_key_1234567890123456")), // Заменить на настоящий секретный ключ
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
app.UseCors("AllowAll");

// Аутентификация и авторизация
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/api/info/users", (UserService service) =>
{
    var users = service.GetAll();
    return Results.Ok(users);
});
app.MapGet("/api/info/websites", (WebsiteService service) =>
{
    var websites = service.GetAll().Select(w => w.URL);
    return Results.Ok(websites);
});

app.Run();