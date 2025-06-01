using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PersonnelTransferRequest.Entities.Models;
using PersonnelTransferRequest.Web.Data;
using PersonnelTransferRequest.Web.Helper;
using PersonnelTransferRequest.Web.Services.DataTable;
using PersonnelTransferRequest.Common;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();


builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddMvc().AddSessionStateTempDataProvider();

// Registering the DataTable service
builder.Services.AddScoped<IDataTableService, DataTableService>();
builder.Services.AddHttpContextAccessor();


builder.Services.ConfigureApplicationCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromMinutes(120); 
    options.SlidingExpiration = true; 
    options.LoginPath = "/giris-yap"; 
    options.LogoutPath = "/Account/Logout"; 
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();

    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseStatusCodePagesWithReExecute("/Home/Error", "?code={0}");
}
else
{

    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseStatusCodePagesWithReExecute("/Home/Error", "?code={0}");
}

app.UseHttpsRedirection();
app.UseStaticFiles();

//check admin user and redirect to admin dashboard
app.MapGet("/admin", async context =>
{
    var signInManager = context.RequestServices.GetRequiredService<SignInManager<ApplicationUser>>();
    var userManager = context.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();

    var user = await userManager.GetUserAsync(context.User);

    if (context.User.Identity?.IsAuthenticated == true && user != null && user.IsAdmin)
    {
        context.Response.Redirect("/Admin/Dashboard");
    }
    else
    {
        context.Response.Redirect("/adminLogin");
    }

    return;
});


app.UseRouting();
app.UseSession();
app.UseAuthorization();



app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);
app.MapRazorPages();

app.Run();
