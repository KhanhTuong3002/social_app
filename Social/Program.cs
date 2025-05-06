using BussinessObject.Entities;
using DataAccess;
using DataAccess.Helpers;
using DataAccess.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddRazorPages();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<SociaDbContex>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Identity Configuration
builder.Services.AddIdentity<User, IdentityRole<string>>()

   .AddEntityFrameworkStores<SociaDbContex>()
   .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Authentication/Login";
    options.AccessDeniedPath = "/Authentication/AccessDenied";
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Auth:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Auth:Google:ClientSecret"];
    options.CallbackPath = "/signin-google";
});
builder.Services.AddAuthorization();


//Services Configuration
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IHashtagServices, HashtagServices>();
builder.Services.AddScoped<IStoriesServices, StoriesServices>();
builder.Services.AddScoped<IFileServices, FileServices>();
builder.Services.AddScoped<IUserServices, UserServices>();


builder.Services.AddScoped<DbContext, SociaDbContex>();

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Database migration and seeding
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SociaDbContex>();
    await context.Database.MigrateAsync();
    await DbInitializer.SeedAsync(context);

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<string>>>();
    await DbInitializer.SeedUserAndRoleAsync(userManager, roleManager);
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // you missed this if you're using Identity
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
