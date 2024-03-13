using Microsoft.AspNetCore.Authentication.Cookies;
using RednitDev.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
//session
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache(); // Required for session state
builder.Services.AddSession(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Recommended for production
        options.IdleTimeout = TimeSpan.FromMinutes(20); // Configure session expiration time
    }
);

builder.Services.AddScoped<AccountService, AccountServiceImpl>();
builder.Services.AddScoped<ManagerService,ManagerServiceImpl>();
//cookies
builder.Services.AddAuthentication(
    CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option => {
        option.LoginPath = "/Access/Login"; //ที่จะใช้สำหรับเข้าสู่ระบบ หากผู้ใช้ไม่ได้รับอนุญาต
        option.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();
app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();