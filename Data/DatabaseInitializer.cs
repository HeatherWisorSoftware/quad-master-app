using Microsoft.EntityFrameworkCore;

namespace QuadMasterApp.Data
{
    public static class DatabaseInitializer
    {
        public static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider, ILogger logger, bool resetDatabase = false)
        {
            try
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var context = services.GetRequiredService<TournamentContext>();

                    // Option to reset the database
                    if (resetDatabase)
                    {
                        logger.LogWarning("Resetting database as requested!");
                        await context.Database.EnsureDeletedAsync();
                    }

                    bool dbExists = await context.Database.CanConnectAsync();

                    if (!dbExists)
                    {
                        // If database doesn't exist, create it from model without migrations
                        logger.LogInformation("Database does not exist. Creating from model...");
                        await context.Database.EnsureDeletedAsync();
                        await context.Database.EnsureCreatedAsync();
                    }
                    else if (context.Database.GetPendingMigrations().Any())
                    {
                        // Only apply migrations if the database already exists and has pending migrations
                        logger.LogInformation("Applying pending migrations to existing database...");
                        try
                        {
                            await context.Database.MigrateAsync();
                        }
                        catch (Exception migrationEx)
                        {
                            logger.LogError(migrationEx, "Error applying migrations. Continuing without migrations.");
                            // Continue without running migrations
                        }
                    }

                    // Seed the database
                    var seeder = services.GetRequiredService<DatabaseSeeder>();
                    await seeder.SeedDatabaseAsync();
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "An error occurred while initializing the database.");
            }
        }
    }
}