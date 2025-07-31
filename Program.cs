using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
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

builder.Services.AddDbContext<TournamentContext>(options =>
    options.UseSqlite($"Data Source={dbPath}")
           .EnableSensitiveDataLogging() // Log SQLite errors
           .EnableDetailedErrors());

builder.Services.AddScoped<DatabaseSeeder>();
builder.Services.AddMudServices();
builder.Services.AddSingleton<IThemeProvider>(provider =>
    new ThemeProvider(new MudTheme(), false));

    
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
 
// Register your other services
builder.Services.AddSingleton<AppStateService>();

// Configure data protection for different environments
var dataProtectionKey = Environment.GetEnvironmentVariable("DataProtection__Key");
if (!string.IsNullOrEmpty(dataProtectionKey))
{
    builder.Services.AddDataProtection()
        .SetApplicationName("quad-master-app")
        .UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration
        {
            EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
            ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
        })
        .SetDefaultKeyLifetime(TimeSpan.FromDays(365));
}
else
{
    var dataProtectionPath = builder.Environment.IsDevelopment()
        ? Path.Combine(Directory.GetCurrentDirectory(), "keys")
        : "/app/keys";
    Directory.CreateDirectory(dataProtectionPath);
    builder.Services.AddDataProtection()
        .SetApplicationName("quad-master-app")
        .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionPath))
        .SetDefaultKeyLifetime(TimeSpan.FromDays(90));
}
var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();

try
{
    var resetDatabase = builder.Configuration.GetValue<bool>
        ("DatabaseOptions:ResetOnStartup");
        logger.LogInformation("Initializing database" +
            " with ResetOnStartup: {ResetOnStartup}, " +
            "Path: {DbPath}", resetDatabase, dbPath);
        await app.Services.InitializeDatabaseAsync(logger, resetDatabase);
}
catch (Exception ex)
{
    logger.LogError(ex, "Failed to initialize database during startup");
    // Don't crash the app, but log the error for debugging
    throw;
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
app.UseRouting();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();