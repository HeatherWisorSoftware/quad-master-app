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

                    logger.LogInformation("Starting database initialization...");

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
                        try
                        {
                            bool dbExists = await context.Database.CanConnectAsync();
                            logger.LogInformation($"Database connection test: {dbExists}");
                            
                            if (!dbExists)
                            {
                                logger.LogInformation("Database does not exist. Creating from model...");
                                await context.Database.EnsureCreatedAsync();
                                logger.LogInformation("Database created successfully");
                            }
                            else 
                            {
                                // Database exists, but let's verify the schema is complete
                                try
                                {
                                    // Test if we can query the Players table (this will fail if table doesn't exist)
                                    var playerCount = await context.Players.CountAsync();
                                    logger.LogInformation($"Database schema verified. Found {playerCount} players.");
                                    
                                    // Check for pending migrations
                                    if (context.Database.GetPendingMigrations().Any())
                                    {
                                        logger.LogInformation("Applying pending migrations to existing database...");
                                        await context.Database.MigrateAsync();
                                        logger.LogInformation("Migrations applied successfully");
                                    }
                                    else
                                    {
                                        logger.LogInformation("Database exists and is up to date");
                                    }
                                }
                                catch (Exception schemaEx)
                                {
                                    logger.LogWarning(schemaEx, "Database exists but schema is incomplete or corrupted. Recreating...");
                                    await context.Database.EnsureDeletedAsync();
                                    await context.Database.EnsureCreatedAsync();
                                    logger.LogInformation("Database recreated due to schema issues");
                                }
                            }
                        }
                        catch (Exception dbEx)
                        {
                            logger.LogError(dbEx, "Error checking database status. Attempting to create new database...");
                            try
                            {
                                await context.Database.EnsureDeletedAsync();
                                await context.Database.EnsureCreatedAsync();
                                logger.LogInformation("Database created after connection error");
                            }
                            catch (Exception createEx)
                            {
                                logger.LogError(createEx, "Failed to create database");
                                throw;
                            }
                        }
                    }

                    // Seed the database
                    try
                    {
                        var seeder = services.GetRequiredService<DatabaseSeeder>();
                        await seeder.SeedDatabaseAsync();
                        logger.LogInformation("Database seeding completed");
                    }
                    catch (Exception seedEx)
                    {
                        logger.LogError(seedEx, "Error during database seeding");
                        // Don't fail startup for seeding errors, but log them
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while initializing the database.");
                throw; // Re-throw so we can see the error
            }
        }
    }
}