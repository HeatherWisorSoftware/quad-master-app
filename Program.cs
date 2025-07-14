using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using MudBlazor;
using MudBlazor.Services;
using QuadMasterApp.Components;
using QuadMasterApp.Data;
using QuadMasterApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Add any other services your app needs
// builder.Services.AddDbContext<YourDbContext>(...);
// Add MudBlazor if you're using it
// builder.Services.AddMudServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.MapRazorPages();
app.MapBlazorHub();

// FIX: Change this line based on your actual app structure
// Try these options in order:
app.MapFallbackToPage("/"); // Option 1: Default page

// If that doesn't work, try:
// app.MapFallbackToPage("/_Layout"); // Option 2
// Or if you have an Index.razor:
// app.MapFallbackToPage("/Index"); // Option 3

// Use the PORT environment variable
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Run($"http://0.0.0.0:{port}");