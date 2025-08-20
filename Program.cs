using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using MudBlazor.Services;
using QuadMasterApp.Components;
using QuadMasterApp.Data;
using QuadMasterApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure database path for different environments
string dbPath;
string dataProtectionPath;

if (builder.Environment.IsDevelopment())
{
    dbPath = "tournament.db";
    dataProtectionPath = Path.Combine(Directory.GetCurrentDirectory(), "keys");
}
else
{
    // Use persistent volume mount point in production
    // DigitalOcean App Platform: mount a volume to /data
    var dataDir = "/data";
    
    // Fallback to /tmp if /data doesn't exist (for other cloud providers)
    if (!Directory.Exists(dataDir))
    {
        dataDir = "/tmp";
        builder.Services.Configure<ConsoleLifetimeOptions>(opts => opts.SuppressStatusMessages = true);
        // Log warning about ephemeral storage
        Console.WriteLine("WARNING: Using ephemeral storage. Database will be lost on container restart!");
    }
    
    dbPath = Path.Combine(dataDir, "tournament.db");
    dataProtectionPath = Path.Combine(dataDir, "keys");
}

// Only create directory if dbPath actually has a directory component
var dbDirectory = Path.GetDirectoryName(dbPath);
if (!string.IsNullOrEmpty(dbDirectory))
{
    Directory.CreateDirectory(dbDirectory);
}

// Ensure data protection directory exists
Directory.CreateDirectory(dataProtectionPath);

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

// Configure data protection
builder.Services.AddDataProtection()
    .SetApplicationName("quad-master-app")
    .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionPath))
    .SetDefaultKeyLifetime(TimeSpan.FromDays(90));

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();

logger.LogInformation($"Using database path: {dbPath}");

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
    app.UseHsts();
}

if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAntiforgery();
app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();