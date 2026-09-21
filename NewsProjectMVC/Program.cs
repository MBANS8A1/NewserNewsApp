using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using NewsProjectMVC.Models.Db;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("MyNewsContext") ?? throw new InvalidOperationException("Connection string 'MyNewsContext' not found.");

builder.Services.AddDbContext<MyNewsContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Sets the path for the login page. 
        // If an unauthorized user tries to access a protected page, they will be redirected here.
        options.LoginPath = "/Auth/Login";

        // Sets the path for the access denied page.
        options.AccessDeniedPath = "/Auth/Login"; 

        // Sets the expiration time for the cookie.
        options.ExpireTimeSpan = TimeSpan.FromDays(10);

        // Makes the cookie essential for the application to function correctly.
        options.SlidingExpiration = true;
    });


// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

//app.MapControllerRoute(
//    name: "Admin",
//    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
// );
app.MapAreaControllerRoute(
    name: "MyAdmin",
    areaName: "Admin",
    pattern: "Admin/{controller=Home}/{action=Index}/{id?}"
 );


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
