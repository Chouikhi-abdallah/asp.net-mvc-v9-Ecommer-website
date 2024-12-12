using Microsoft.EntityFrameworkCore;
using Proj.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Configure Entity Framework and the database context
builder.Services.AddDbContext<EcommerceDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add distributed memory cache for session state
builder.Services.AddDistributedMemoryCache();

// Add session services with custom configuration
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout
    options.Cookie.HttpOnly = true; // Secure session cookie
    options.Cookie.IsEssential = true; // Make the session cookie essential for GDPR compliance
});

// Build the app
var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Enable HTTPS redirection
//app.UseHttpsRedirection();

// Enable serving static files
app.UseStaticFiles();

// Use session middleware
app.UseSession();

// Configure routing
app.UseRouting();

// Map controllers to routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Run the application
app.Run();