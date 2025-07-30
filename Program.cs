using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using MudBlazor.Services;
using QuadMasterApp.Components;
using QuadMasterApp.Data;
using QuadMasterApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure database path for different environments
var dbPath = builder.Environment.IsDevelopment() 
    ? "tournament.db" 
    : Path.Combine("/app", "tournament.db");

// Single DbContext registration
builder.Services.AddDbContext<TournamentContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddScoped<DatabaseSeeder>();

// Add MudBlazor services
builder.Services.AddMudServices();

// Register ThemeProvider with factory
builder.Services.AddSingleton<IThemeProvider>(provider =>
    new ThemeProvider(new MudTheme(), false));

    
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
 
// Register your other services
builder.Services.AddSingleton<AppStateService>();

// Configure data protection for different environments
var dataProtectionPath = builder.Environment.IsDevelopment() 
    ? Path.Combine(Directory.GetCurrentDirectory(), "keys")
    : "/tmp/keys";

// Ensure the directory exists
Directory.CreateDirectory(dataProtectionPath);

builder.Services.AddDataProtection()
    .SetApplicationName("quad-master-app")
    .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionPath))
    .SetDefaultKeyLifetime(TimeSpan.FromDays(90));

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();

try
{
    // Get database reset configuration from appsettings.json
    var resetDatabase = builder.Configuration.GetValue<bool>("DatabaseOptions:ResetOnStartup");

    // Initialize the database with the configuration setting
    await app.Services.InitializeDatabaseAsync(logger, resetDatabase);
}
catch (Exception ex)
{
    logger.LogError(ex, "Failed to initialize database during startup");
    // Don't crash the app, but log the error for debugging
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Only use HTTPS redirection in development
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
//app.UseRouting();
app.UseAntiforgery();
app.MapStaticAssets();

// Route to your Components
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();