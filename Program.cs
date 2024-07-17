using Microsoft.EntityFrameworkCore;
using Proyecto_TiendaElectronica.ModelBinder;
using Proyecto_TiendaElectronica.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add services to the container.
builder.Services.AddControllersWithViews(
    options => {
        options.ModelBinderProviders.Insert(0, new CustomModelBinderProvider());
    }
);

var connectionString = builder.Configuration.GetConnectionString("Server=localhost;Database=TiendaElectronica;Trusted_Connection=True;TrustServerCertificate=True;");

builder.Services.AddDbContext<AppDBContext>(options => options.UseSqlServer(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Admin}/{action=Index}/{id?}");

app.Run();
