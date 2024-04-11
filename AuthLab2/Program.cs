using AuthLab2.Data;
using AuthLab2.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var services = builder.Services;
        var configuration = builder.Configuration;

        services.AddAuthentication().AddGoogle(googleOptions =>
        {
            googleOptions.ClientId = configuration["Authentication:Google:ClientId"];
            googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"];
        });

        services.Configure<CookiePolicyOptions>(options =>
        {
            options.CheckConsentNeeded = context => true; // This lambda determines whether user consent is needed for this request.
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });

        // Add services to the container.
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddControllersWithViews();

        services.AddDbContext<LegoContext>(options =>
        {
            options.UseSqlServer(configuration["ConnectionStrings:DefaultConnection"]);
        });

        services.AddScoped<ILegoRepository, EFLegoRepository>();

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

        app.UseCookiePolicy();

        app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

        app.MapControllerRoute("pagenumandtype", "{bookType}/{pageNum}", new { Controller = "Home", action = "Index" });
        app.MapControllerRoute("pagination", "{pageNum}", new { Controller = "Home", action = "Index", pageNum = 1 });
        app.MapControllerRoute("bookType", "{bookType}", new { Controller = "Home", action = "Index", pageNum = 1 });


        app.MapDefaultControllerRoute();
        app.MapRazorPages();

        using (var scope = app.Services.CreateScope())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var roles = new[] { "Admin", "Customer", "Visitor" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        using (var scope = app.Services.CreateScope())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Seed the "Visitor" role if it doesn't exist
            if (!await roleManager.RoleExistsAsync("Visitor"))
            {
                var role = new IdentityRole("Visitor");
                await roleManager.CreateAsync(role);
            }
        }

        using (var scope = app.Services.CreateScope())
        {
            var userManager = 
                scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            string email = "aurora@gmail.com";
            string password = "Bricks123!";

            if (await userManager.FindByEmailAsync(email)==null)
            {
                var user = new IdentityUser();
                user.UserName = email;
                user.Email = email;

                await userManager.CreateAsync(user, password);

                await userManager.AddToRoleAsync(user, "Admin");

            }
        }

        using (var scope = app.Services.CreateScope())
        {
            var userManager =
                scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            string email = "aurora@gmail.com";
            string password = "Bricks123!";

            if (await userManager.FindByEmailAsync(email) == null)
            {
                var user = new IdentityUser();
                user.UserName = email;
                user.Email = email;

                await userManager.CreateAsync(user, password);

                await userManager.AddToRoleAsync(user, "Admin");

            }
        }

        app.Run();
    }
}