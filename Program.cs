using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
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

// Register ThemeProvider as a singleton service
// Create a default MudTheme to initialize the provider
// The actual theme will be set by MainLayout when it initializes
var defaultTheme = new MudTheme();
builder.Services.AddSingleton<IThemeProvider>(serviceProvider =>
    new ThemeProvider(defaultTheme, false));

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add global app state
builder.Services.AddSingleton<AppStateService>();

// Add Microsoft Identity authentication
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

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

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .RequireAuthorization("AllowedUsers");

#if RELEASE
app.Urls.Add("http://localhost:5000");
app.Urls.Add("https://localhost:5001");
#endif

app.Run();
