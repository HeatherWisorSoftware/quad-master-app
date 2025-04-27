using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuadMasterApp.Data;


namespace QuadMasterApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddControllers();

            builder.Services.AddDbContext<TournamentContext>(options =>
                options.UseSqlite("Data Source = tournament.db"));

            var app = builder.Build();

            if (!app.Environment.IsProduction())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();
            app.MapControllers();
            app.Run();
        }
    }
}
