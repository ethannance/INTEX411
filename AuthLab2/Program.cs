using AuthLab2.Data;
using AuthLab2.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddAuthentication().AddGoogle(googleOptions =>
{
    googleOptions.ClientId = configuration["Authentication:Google:ClientId"];
    googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"];
});

// Add services to the container.
var connectionString = configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

services.AddDatabaseDeveloperPageExceptionFilter();

services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

services.AddControllersWithViews();

services.AddDbContext<BookstoreContext>(options =>
{
    options.UseSqlite(configuration["ConnectionStrings:BookConnection"]);
});

services.AddScoped<IBookRepository, EFBookRepository>();

services.AddRazorPages();

services.AddDistributedMemoryCache();
services.AddSession(); // Register session services

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); // Add UseSession middleware

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute("pagenumandtype", "{bookType}/{pageNum}", new { Controller = "Home", action = "Index" });
app.MapControllerRoute("pagination", "{pageNum}", new { Controller = "Home", action = "Index", pageNum = 1 });
app.MapControllerRoute("bookType", "{bookType}", new { Controller = "Home", action = "Index", pageNum = 1 });


app.MapDefaultControllerRoute();
app.MapRazorPages();

app.Run();
