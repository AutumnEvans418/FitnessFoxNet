using FitnessFox.Components;
using FitnessFox.Components.Account;
using FitnessFox.Components.Services;
using FitnessFox.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddMudServices();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var dbType = builder.Configuration["DatabaseType"];

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlite("Data Source=database.dat").EnableSensitiveDataLogging().EnableDetailedErrors();
});
builder.Services.AddDbContext<IdentityDbContext>(options =>
{
    options.UseSqlite("Data Source=identity.dat").EnableSensitiveDataLogging().EnableDetailedErrors();
});

builder.Services.RegisterComponentDependencies();
builder.Services.AddScoped<IFileService, FileService>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<IdentityDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<IdentityUser>, IdentityNoOpEmailSender>();

var app = builder.Build();

using var scope = app.Services.CreateScope();

var idDb = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

var clearDb = builder.Configuration.GetValue<bool>("ClearDatabaseOnStartup");

if (clearDb)
{
    await idDb.Database.EnsureDeletedAsync();
    await db.Database.EnsureDeletedAsync();
}
await idDb.Database.EnsureCreatedAsync();
await db.Database.EnsureCreatedAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddAdditionalAssemblies(typeof(FitnessFox.Components.Components.Layout.MainLayout).Assembly)
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
