using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using MudBlazor.Services;
using QuadMasterApp.Components;
using QuadMasterApp.Data;
using QuadMasterApp.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

var logger = LoggerFactory.Create(config => config.AddConsole()).CreateLogger<Program>();
logger.LogInformation("Starting application...");

// Configure database path for different environments
var dbPath = builder.Environment.IsDevelopment() 
    ? "tournament.db" 
    : Path.Combine("/app", "tournament.db");

logger.LogInformation($"Using database path: {dbPath}");

// Single DbContext registration
builder.Services.AddDbContext<TournamentContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddScoped<DatabaseSeeder>();
logger.LogInformation("Add Scoped DatabaseSeeder");

// Add MudBlazor services
builder.Services.AddMudServices();
logger.LogInformation("AddMudServices");


// Register ThemeProvider with factory
builder.Services.AddSingleton<IThemeProvider>(provider =>
    new ThemeProvider(new MudTheme(), false));

    
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register your other services
builder.Services.AddScoped<AppStateService>();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDataProtection();
    logger.LogInformation("IsDevelopment, AddDataProtection");
}
else
{
    builder.Services.AddDataProtection()
        .SetApplicationName("quad-master-app");
    logger.LogInformation("IsProduction, AddDataProtection");
}

var app = builder.Build();

try
{
    // Get database reset configuration from appsettings.json
    var resetDatabase = builder.Configuration.GetValue<bool>("DatabaseOptions:ResetOnStartup");
    logger.LogInformation("Before InitializeDatabaseAsync({resetDatabase})", resetDatabase);

    // Initialize the database with the configuration setting
    await app.Services.InitializeDatabaseAsync(logger, resetDatabase);
    logger.LogInformation("After InitializeDatabaseAsync({resetDatabase})", resetDatabase);
}
catch (Exception ex)
{
    logger.LogError(ex, "Failed to initialize database during startup");
    // Don't crash the app, but log the error for debugging
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    logger.LogInformation("Before UseExceptionHandler");
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    // Only use HTTPS redirection in development
    logger.LogInformation("Before UseHsts");
    app.UseHsts();

}

logger.LogInformation("Before UseStaticFiles");
app.UseStaticFiles();
//app.UseRouting();
logger.LogInformation("Before UseAntiforgery");
app.UseAntiforgery();
logger.LogInformation("Before MapStaticAssets");
app.MapStaticAssets();

// Route to your Components
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

logger.LogInformation("After MapRazorComponents");
app.Run();