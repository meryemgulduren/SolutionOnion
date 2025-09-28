using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SO.Application;
using SO.Application.Abstractions.Services;
using SO.Application.Abstractions.Services.AccountModule;
using SO.Domain.Entities.Identity;
using SO.Infrastructure;
using SO.Infrastructure.Services;
using SO.Persistence;
using SO.Persistence.Contexts;
using SO.Web.Models; 
using SO.Web.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// HttpContextAccessor Application katmanında ekleniyor

builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddInfrastructureServices();
builder.Services.AddApplicationServices();

// HATA İÇİN EKLENEN SATIR: IDocumentExportService için servis kaydı
// NOT: 'DocumentExportService' ismini kendi sınıf adınızla değiştirmeyi unutmayın.
builder.Services.AddScoped<IDocumentExportService, DocumentExportService>();

// Email settings & service
builder.Services.Configure<SO.Infrastructure.Services.EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, SO.Infrastructure.Services.EmailService>();

// Permission DI
builder.Services.Configure<PermissionOptions>(builder.Configuration.GetSection("PermissionOptions"));
builder.Services.AddScoped<IAppAuthorizationService, AppAuthorizationService>();


// Identity configuration
builder.Services.AddIdentity<AppUser, AppRole>(options =>
{
    // Şifre gereksinimleri
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;

    // Kullanıcı gereksinimleri
    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

    // Kilitleme (Lockout) ayarları
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // Giriş gereksinimleri
    options.SignIn.RequireConfirmedEmail = true;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddEntityFrameworkStores<SODbContext>()
.AddDefaultTokenProviders();

// Cookie configuration
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(2);
    options.SlidingExpiration = true;
    options.Cookie.Name = ".SO.Auth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

var app = builder.Build();

// Database seeding
using (var scope = app.Services.CreateScope())
{
    var databaseSeeder = scope.ServiceProvider.GetRequiredService<SO.Persistence.Seed.DatabaseSeeder>();
    await databaseSeeder.SeedAsync();
}

// Configure the HTTP request pipeline.
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
