using DriveHub.Web.Data;
using DriveHub.Web.Services.Facades;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// База данных
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Сервисы приложения
builder.Services.AddScoped<IDealFacade, DealFacade>();

// MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();