using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using QuadMasterApp.Components;
using QuadMasterApp.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TournamentContext>(options =>
    options.UseSqlite("Data Source = tournament.db"));

builder.Services.AddScoped<DatabaseSeeder>();

// Add MudBlazor services
builder.Services.AddMudServices();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

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


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
