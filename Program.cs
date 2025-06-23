using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using MudBlazor;
using MudBlazor.Services;
using QuadMasterApp.Components;
using QuadMasterApp.Data;
using QuadMasterApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure database connection with proper Azure path
builder.Services.AddDbContext<TournamentContext>(options =>
{
    string dbPath;
    if (builder.Environment.IsDevelopment())
    {
        // Local development
        dbPath = "Data Source=tournament.db";
    }
    else
    {
        // Azure App Service - use writable persistent storage
        var dataDir = "/home/data";
        Directory.CreateDirectory(dataDir);
        dbPath = $"Data Source={dataDir}/tournament.db";
    }

    options.UseSqlite(dbPath);
});

builder.Services.AddScoped<DatabaseSeeder>();

// Add MudBlazor services
builder.Services.AddMudServices();

// Register ThemeProvider as a singleton service
var defaultTheme = new MudTheme();
builder.Services.AddSingleton<IThemeProvider>(serviceProvider =>
    new ThemeProvider(defaultTheme, false));

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add global app state
builder.Services.AddSingleton<AppStateService>();

// Add authentication only in production
if (!builder.Environment.IsDevelopment())
{
    // Add Microsoft Identity authentication
    builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
        .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));
}

// Add authorization with email restrictions
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AllowedUsers", policy =>
        policy.RequireAssertion(context =>
        {
            // List of allowed email addresses
            var allowedEmails = new[]
            {
                "heather@wisorsoftware.com",
                "bobscaccia62@gmail.com",
                "shawnie@wisorsoftware.com",
                "bob.scaccia@usafirmware.com",
                "jason.hoagland.jr@gmail.com"
            };

            // Get the user's email from the authentication claims
            var userEmail = context.User.FindFirst("preferred_username")?.Value
                            ?? context.User.FindFirst("email")?.Value
                            ?? context.User.Identity?.Name;

            // Check if user's email is in the allowed list
            return allowedEmails.Contains(userEmail, StringComparer.OrdinalIgnoreCase);
        }));
});

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();

try
{
    logger.LogInformation("Starting database initialization...");
    var resetDatabase = builder.Configuration.GetValue<bool>("DatabaseOptions:ResetOnStartup");
    logger.LogInformation("Reset database setting: {ResetDatabase}", resetDatabase);

    await app.Services.InitializeDatabaseAsync(logger, resetDatabase);
    logger.LogInformation("Database initialized successfully");
}
catch (Exception ex)
{
    logger.LogError(ex, "Failed to initialize database: {Message}", ex.Message);
    throw; // Re-throw to see the actual error
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // Azure handles HTTPS termination
    app.UseForwardedHeaders();
    // Temporarily show detailed errors for debugging
    app.UseDeveloperExceptionPage();
    //app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();

// Use authentication only in production
if (!app.Environment.IsDevelopment())
{
    app.UseAuthentication();
}

app.UseAuthorization();
app.UseAntiforgery();

app.MapStaticAssets();

// Configure components with conditional authentication
if (!app.Environment.IsDevelopment())
{
    // Production: Require authentication
    app.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode()
        .RequireAuthorization("AllowedUsers");
}
else
{
    // Development: No authentication
    app.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode();
}

app.Run();