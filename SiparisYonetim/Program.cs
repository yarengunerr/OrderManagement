using Microsoft.EntityFrameworkCore;
using SiparisYonetim.Data;
using SiparisYonetim.Hubs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddRazorPages();


builder.Services.AddDistributedMemoryCache(); 
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); 
    options.Cookie.HttpOnly = true; 
    options.Cookie.IsEssential = true;
});


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer("Server=GUNER\\SQLEXPRESS01;Database=SiparisYonetimDB;Trusted_Connection=True;TrustServerCertificate=True"));


builder.Services.AddSignalR();


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";  
        options.AccessDeniedPath = "/AccessDenied";  
    });

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession(); 
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();


app.MapRazorPages();


app.MapHub<ProductHub>("/productHub");

app.Run();
