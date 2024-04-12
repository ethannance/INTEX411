using AuthLab2.Data;
using AuthLab2.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


internal class Program
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMvc();
        services.AddAntiforgery(options => options.HeaderName = "X-CSRF-TOKEN");
    }
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var services = builder.Services;
        var configuration = builder.Configuration;

        services.AddAuthentication().AddMicrosoftAccount(microsoftOptions =>
        {
            microsoftOptions.ClientId = configuration["Authentication:Microsoft:ClientId"];
            microsoftOptions.ClientSecret = configuration["Authentication:Microsoft:ClientSecret"];
        });

        services.Configure<CookiePolicyOptions>(options =>
        {
            options.CheckConsentNeeded = context => true; // This lambda determines whether user consent is needed for this request.
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });

        // AZURE CONNECTION STRING.
        var connectionString = configuration.GetConnectionString("LegoDBConnectionString")
            ?? throw new InvalidOperationException("The connection string 'LegoDBConnectionString' was not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddDbContext<LegoContext>(options =>
            options.UseSqlServer(connectionString));

        //

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
            app.UseHsts(); //HSTS
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.Use(async (context, next) =>
        {
            context.Response.Headers.Add("Content-Security-Policy", //CSP Header
                "default-src 'self' * data: blob:;" +
                "script-src 'self' * 'unsafe-inline' 'unsafe-eval' 'https://use.fontawesome.com' 'https://ajax.googleapis.com';" +
                "style-src 'self' * 'https://fonts.googleapis.com' 'unsafe-inline';" +
                "img-src 'self' * data: blob:;" +
                "font-src 'self' * data: 'https://fonts.gstatic.com' 'https://use.fontawesome.com';" +
                "connect-src 'self' *;" +
                "object-src 'self' *;" +
                "frame-src 'self' *;");
            await next();
        });


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