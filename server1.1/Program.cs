using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using server1._1.Controllers;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Указываем путь к базе данных
var dbPath = Path.Combine(AppContext.BaseDirectory, "users.db");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

// 🔹 Подключаем контроллеры с настройкой сериализации DateTime
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 🔹 Настройка CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

var app = builder.Build();

// 🔹 Применяем миграции и добавляем администратора
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    // Очистка пустых записей
    CleanupService.CleanEmptyRecords(db);

    // 🔹 Добавляем роль, отдел и должность, если их нет
    var adminRole = db.UserRoles.FirstOrDefault(r => r.Name == "Админ") ?? new UserRole { Name = "Админ" };
    var adminDept = db.Departments.FirstOrDefault(d => d.Name == "ИТ-отдел") ?? new Department { Name = "ИТ-отдел" };
    var adminPos = db.Positions.FirstOrDefault(p => p.Title == "Администратор систем") ?? new Position { Title = "Администратор систем" };

    if (adminRole.Id == 0) db.UserRoles.Add(adminRole);
    if (adminDept.Id == 0) db.Departments.Add(adminDept);
    if (adminPos.Id == 0) db.Positions.Add(adminPos);
    db.SaveChanges();

    // 🔹 Добавляем администратора, если ещё нет
    if (!db.Employees.Any(e => e.Username == "admin"))
    {
        string password = "admin";
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        string passwordHash = Convert.ToBase64String(hash);

        var admin = new Employee
        {
            FullName = "Главный администратор",
            Username = "admin",
            PasswordHash = passwordHash,
            RoleId = adminRole.Id,
            DepartmentId = adminDept.Id,
            PositionId = adminPos.Id
        };

        db.Employees.Add(admin);
        db.SaveChanges();
    }
}

// 🔹 Подключаем CORS
app.UseCors("AllowAll");

// 🔹 Подключаем Swagger
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Admin API V1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();


// Кастомный JSON-конвертер для DateTime → yyyy-MM-dd
public class DateOnlyJsonConverter : JsonConverter<DateTime>
{
    private const string Format = "yyyy-MM-dd"; //  формат, как ты хочешь

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var raw = reader.GetString();
        var parsed = DateTime.Parse(raw!);         //поддерживает дату с временем
        return parsed.Date;                        // возвращаем только часть с датой (время = 00:00:00)
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(Format)); //  сохраняем как yyyy-MM-dd
    }
}

