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
                        await context.Database.EnsureCreatedAsync();
                        logger.LogInformation("Database reset and recreated");
                    }
                    else
                    {
                        bool dbExists = await context.Database.CanConnectAsync();
                        if (!dbExists)
                        {
                            logger.LogInformation("Database does not exist. Creating from model...");
                            await context.Database.EnsureCreatedAsync();
                        }
                        else if (context.Database.GetPendingMigrations().Any())
                        {
                            logger.LogInformation("Applying pending migrations to existing database...");
                            try
                            {
                                await context.Database.MigrateAsync();
                            }
                            catch (Exception migrationEx)
                            {
                                logger.LogError(migrationEx, "Error applying migrations. Recreating database from model...");
                                // If migrations fail, recreate the database
                                await context.Database.EnsureDeletedAsync();
                                await context.Database.EnsureCreatedAsync();
                            }
                        }
                        else
                        {
                            // Database exists and no pending migrations
                            logger.LogInformation("Database exists and is up to date");
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
                throw; // Re-throw so we can see the error
            }
        }
    }
}