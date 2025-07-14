using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using MudBlazor;
using MudBlazor.Services;
using QuadMasterApp.Components;
using QuadMasterApp.Data;
using QuadMasterApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services for Blazor Web App
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add MudBlazor services
builder.Services.AddMudServices();

// Register ThemeProvider with factory (handles constructor requirements)
builder.Services.AddScoped<IThemeProvider>(provider =>
    new ThemeProvider(new MudTheme(), false));

// Register your other services
builder.Services.AddScoped<AppStateService>();
builder.Services.AddScoped<DatabaseSeeder>();

// Add database context
builder.Services.AddDbContext<TournamentContext>(options =>
    options.UseSqlite("Data Source=tournament.db"));

var app = builder.Build();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<TournamentContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        await context.Database.EnsureCreatedAsync();

        var seeder = services.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedDatabaseAsync();

        logger.LogInformation("Database initialized successfully");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing the database.");
    }
}

// Configure pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

// Route to your Components
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();