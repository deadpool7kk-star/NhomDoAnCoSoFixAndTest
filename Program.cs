using Book2.Data;
using Book2.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Book2.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<ILogService, LogService>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                      ?? throw new InvalidOperationException("Không tìm thấy chuỗi kết nối 'DefaultConnection'.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;

    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        var googleAuthNSection = builder.Configuration.GetSection("Authentication:Google");
        options.ClientId = googleAuthNSection["ClientId"] ?? throw new InvalidOperationException("Google ClientId not found.");
        options.ClientSecret = googleAuthNSection["ClientSecret"] ?? throw new InvalidOperationException("Google ClientSecret not found.");
    });

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

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

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context =
        services.GetRequiredService<ApplicationDbContext>();

    context.Database.EnsureCreated();

    KhoiTaoDuLieu.Seed(context);

    await SeedRole.InitializeAsync(services);
}

app.Run();