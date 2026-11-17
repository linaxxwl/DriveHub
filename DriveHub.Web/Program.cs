using DriveHub.Web.Data;
using DriveHub.Web.Services;
using DriveHub.Web.Services.Facades;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ЛОГИРОВАНИЕ: пишем логи в папку logs
Log.Logger = new LoggerConfiguration()
    .WriteTo.File("logs/drivehub-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Host.UseSerilog();

// БАЗА ДАННЫХ: SQL Server (строка подключения в appsettings.json)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// СЕРВИСЫ ПРИЛОЖЕНИЯ (внедрение зависимостей)
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<CatalogService>();
builder.Services.AddScoped<IDealFacade, DealFacade>();
builder.Services.AddScoped<FavoritesService>();
builder.Services.AddScoped<RequestService>();

// АУТЕНТИФИКАЦИЯ: вход через cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath = "/Account/Login";
        o.AccessDeniedPath = "/Account/Login";
    });

// MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ПРИ СТАРТЕ: применяем миграции и добавляем начальные данные
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    await DbSeeder.SeedAsync(db);
}

// ОБРАБОТКА ЗАПРОСОВ
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // кто ты?
app.UseAuthorization();  // что тебе можно?

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();