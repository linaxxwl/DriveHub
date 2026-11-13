using DriveHub.Web.Data;
using DriveHub.Web.Services.Facades;
using DriveHub.Web.Services.Reports;
using Microsoft.EntityFrameworkCore;
using Serilog;
using DriveHub.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;


var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.File("logs/drivehub-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Host.UseSerilog();

// База данных
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Сервисы приложения
builder.Services.AddScoped<IDealFacade, DealFacade>();

// MVC
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IReportExporter, CsvReportExporter>();
builder.Services.AddScoped<IReportExporter, ExcelReportAdapter>();

builder.Services.AddScoped<AccountService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o => { o.LoginPath = "/Account/Login"; o.AccessDeniedPath = "/Account/Login"; });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();