using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using MudBlazor.Services;
using QuadMasterApp.Components;
using QuadMasterApp.Data;
using QuadMasterApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TournamentContext>(options =>
    options.UseSqlite("Data Source = tournament.db"));

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

// Add database context
builder.Services.AddDbContext<TournamentContext>(options =>
    options.UseSqlite("Data Source=tournament.db"));

// Add data protection configuration
builder.Services.AddDataProtection()
    .SetApplicationName("quad-master-app")
    .PersistKeysToFileSystem(new DirectoryInfo("/app/keys"))
    .SetDefaultKeyLifetime(TimeSpan.FromDays(90));

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();

// Get database reset configuration from appsettings.json
var resetDatabase = builder.Configuration.GetValue<bool>("DatabaseOptions:ResetOnStartup");

// Initialize the database with the configuration setting
await app.Services.InitializeDatabaseAsync(logger, resetDatabase);

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