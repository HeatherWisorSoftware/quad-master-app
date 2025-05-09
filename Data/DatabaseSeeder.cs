using Microsoft.EntityFrameworkCore;
using QuadMasterApp.Data;
using QuadMasterApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuadMasterApp.Data
{
    public class DatabaseSeeder
    {
        private readonly TournamentContext _context;
        private readonly Random _random = new Random(123); // Use fixed seed for reproducibility

        public DatabaseSeeder(TournamentContext context)
        {
            _context = context;
        }

        public async Task SeedDatabaseAsync()
        {
            // Only seed if the database is empty
            if (!_context.Players.Any() && !_context.Tournaments.Any())
            {
                // Seed players
                await SeedPlayersAsync();

                // Seed tournaments
                await SeedTournamentsAsync();

                // Seed tournament players
                await SeedTournamentPlayersAsync();

                // Seed quads for some tournaments
                await SeedQuadsAsync();

                Console.WriteLine("Database seeding completed successfully!");
            }
            else
            {
                Console.WriteLine("Database already contains data. Skipping seeding process.");
            }
        }

        private async Task SeedPlayersAsync()
        {
            var players = new List<Player>
            {
                new Player { FirstName = "John", LastName = "Smith", Ranking = 1840, Email = "john.smith@example.com", Phone = "555-123-4567" },
                new Player { FirstName = "David", LastName = "Johnson", Ranking = 1985, Email = "david.johnson@example.com", Phone = "555-234-5678" },
                new Player { FirstName = "Michael", LastName = "Williams", Ranking = 2105, Email = "michael.williams@example.com", Phone = "555-345-6789" },
                new Player { FirstName = "James", LastName = "Brown", Ranking = 1750, Email = "james.brown@example.com", Phone = "555-456-7890" },
                new Player { FirstName = "FM", LastName = "Robert Davis", Ranking = 2270, Email = "robert.davis@example.com", Phone = "555-567-8901" },
                new Player { FirstName = "Daniel", LastName = "Miller", Ranking = 1890, Email = "daniel.miller@example.com", Phone = "555-678-9012" },
                new Player { FirstName = "William", LastName = "Wilson", Ranking = 1910, Email = "william.wilson@example.com", Phone = "555-789-0123" },
                new Player { FirstName = "Richard", LastName = "Moore", Ranking = 2050, Email = "richard.moore@example.com", Phone = "555-890-1234" },
                new Player { FirstName = "Joseph", LastName = "Taylor", Ranking = 1795, Email = "joseph.taylor@example.com", Phone = "555-901-2345" },
                new Player { FirstName = "Thomas", LastName = "Anderson", Ranking = 1875, Email = "thomas.anderson@example.com", Phone = "555-012-3456" },
                new Player { FirstName = "Charles", LastName = "Jackson", Ranking = 1950, Email = "charles.jackson@example.com", Phone = "555-123-4568" },
                new Player { FirstName = "Christopher", LastName = "White", Ranking = 2125, Email = "christopher.white@example.com", Phone = "555-234-5679" },
                new Player { FirstName = "Edward", LastName = "Harris", Ranking = 1830, Email = "edward.harris@example.com", Phone = "555-345-6780" },
                new Player { FirstName = "Brian", LastName = "Martin", Ranking = 1920, Email = "brian.martin@example.com", Phone = "555-456-7891" },
                new Player { FirstName = "Louis", LastName = "Zhang", Ranking = 1959, Email = "louis.zhang@example.com", Phone = "555-567-8902" },
                new Player { FirstName = "Jonas", LastName = "M Reed Jr.", Ranking = 1959, Email = "jonas.reed@example.com", Phone = "555-678-9013" },
                new Player { FirstName = "Ting", LastName = "Chieh", Ranking = 1927, Email = "ting.chieh@example.com", Phone = "555-789-0124" },
                new Player { FirstName = "FM", LastName = "Aleksa Micic", Ranking = 2278, Email = "aleksa.micic@example.com", Phone = "555-890-1235" },
                new Player { FirstName = "Daniel", LastName = "Wang", Ranking = 2100, Email = "daniel.wang@example.com", Phone = "555-901-2346" },
                new Player { FirstName = "Kevin", LastName = "Thompson", Ranking = 1865, Email = "kevin.thompson@example.com", Phone = "555-012-3457" },
                new Player { FirstName = "Jason", LastName = "Garcia", Ranking = 1975, Email = "jason.garcia@example.com", Phone = "555-123-4569" },
                new Player { FirstName = "Ronald", LastName = "Martinez", Ranking = 1820, Email = "ronald.martinez@example.com", Phone = "555-234-5680" },
                new Player { FirstName = "Jeffrey", LastName = "Robinson", Ranking = 1900, Email = "jeffrey.robinson@example.com", Phone = "555-345-6781" },
                new Player { FirstName = "Amanda", LastName = "Lewis", Ranking = 1780, Email = "amanda.lewis@example.com", Phone = "555-456-7892" },
                new Player { FirstName = "Nicole", LastName = "Lee", Ranking = 1850, Email = "nicole.lee@example.com", Phone = "555-567-8903" },
                new Player { FirstName = "Sarah", LastName = "Walker", Ranking = 1925, Email = "sarah.walker@example.com", Phone = "555-678-9014" },
                new Player { FirstName = "Elizabeth", LastName = "Hall", Ranking = 1975, Email = "elizabeth.hall@example.com", Phone = "555-789-0125" },
                new Player { FirstName = "Robert", LastName = "Johnson", Ranking = 2050, Email = "robert.johnson@example.com", Phone = "555-890-1236" },
                new Player { FirstName = "Andrew", LastName = "Lee", Ranking = 1750, Email = "andrew.lee@example.com", Phone = "555-901-2347" },
                new Player { FirstName = "Steven", LastName = "Chen", Ranking = 1885, Email = "steven.chen@example.com", Phone = "555-012-3458" },
                new Player { FirstName = "Matthew", LastName = "Young", Ranking = 1935, Email = "matthew.young@example.com", Phone = "555-123-4570" },
                new Player { FirstName = "Anthony", LastName = "Wright", Ranking = 2010, Email = "anthony.wright@example.com", Phone = "555-234-5681" }
            };

            _context.Players.AddRange(players);
            await _context.SaveChangesAsync();
            Console.WriteLine($"Added {players.Count} players to the database.");
        }

        private async Task SeedTournamentsAsync()
        {
            var tournaments = new List<Tournament>
            {
                new Tournament
                {
                    Name = "Chess Masters 2025",
                    Date = DateTime.Now.AddDays(30),
                    VenueName = "Downtown Chess Club",
                    Street = "123 Main Street",
                    City = "New York",
                    State = "NY",
                    PostalCode = "10001",
                    Country = "USA",
                    FormattedAddress = "123 Main Street, New York, NY 10001, USA"
                },
                new Tournament
                {
                    Name = "Spring Chess Tournament",
                    Date = DateTime.Now.AddDays(15),
                    VenueName = "Community Center",
                    Street = "456 Park Avenue",
                    City = "Boston",
                    State = "MA",
                    PostalCode = "02108",
                    Country = "USA",
                    FormattedAddress = "456 Park Avenue, Boston, MA 02108, USA"
                },
                new Tournament
                {
                    Name = "City Championship",
                    Date = DateTime.Now.AddDays(60),
                    VenueName = "Golden Gate Chess Club",
                    Street = "789 Market Street",
                    City = "San Francisco",
                    State = "CA",
                    PostalCode = "94103",
                    Country = "USA",
                    FormattedAddress = "789 Market Street, San Francisco, CA 94103, USA"
                },
                new Tournament
                {
                    Name = "Junior Chess Open",
                    Date = DateTime.Now.AddDays(45),
                    VenueName = "Westside School",
                    Street = "101 Westwood Blvd",
                    City = "Los Angeles",
                    State = "CA",
                    PostalCode = "90024",
                    Country = "USA",
                    FormattedAddress = "101 Westwood Blvd, Los Angeles, CA 90024, USA"
                },
                new Tournament
                {
                    Name = "Winter Chess Festival",
                    Date = DateTime.Now.AddDays(90),
                    VenueName = "Lake View Hotel",
                    Street = "222 Lakeshore Drive",
                    City = "Chicago",
                    State = "IL",
                    PostalCode = "60611",
                    Country = "USA",
                    FormattedAddress = "222 Lakeshore Drive, Chicago, IL 60611, USA"
                }
            };

            _context.Tournaments.AddRange(tournaments);
            await _context.SaveChangesAsync();
            Console.WriteLine($"Added {tournaments.Count} tournaments to the database.");
        }

        private async Task SeedTournamentPlayersAsync()
        {
            var players = await _context.Players.ToListAsync();
            var tournaments = await _context.Tournaments.ToListAsync();

            foreach (var tournament in tournaments)
            {
                // Randomly assign 20-30 players to each tournament
                var playerCount = _random.Next(20, Math.Min(31, players.Count + 1));
                var selectedPlayers = players.OrderBy(x => _random.Next()).Take(playerCount).ToList();

                foreach (var player in selectedPlayers)
                {
                    var tournamentPlayer = new TournamentPlayer
                    {
                        PlayerId = player.Id,
                        TournamentId = tournament.Id,
                        Round1Score = "",
                        Round2Score = "",
                        Round3Score = "",
                        Round1Opponent = "",
                        Round2Opponent = "",
                        Round3Opponent = ""
                        // QuadId will be assigned later in SeedQuadsAsync
                    };

                    _context.TournamentPlayers.Add(tournamentPlayer);
                }
            }

            await _context.SaveChangesAsync();
            Console.WriteLine("Tournament players added successfully.");
        }

        private async Task SeedQuadsAsync()
        {
            // Get the first two tournaments to create quads for
            var tournaments = await _context.Tournaments.Take(2).ToListAsync();

            foreach (var tournament in tournaments)
            {
                var tournamentPlayers = await _context.TournamentPlayers
                    .Include(tp => tp.Player)
                    .Where(tp => tp.TournamentId == tournament.Id)
                    .OrderByDescending(tp => tp.Player.Ranking)
                    .ToListAsync();

                // Calculate how many complete quads we can create (groups of 4 players)
                int completeQuads = tournamentPlayers.Count / 4;

                // Create quad groups - each group can have up to 4 quads (16 players)
                int currentGroup = 1;
                int quadCounter = 0;

                for (int i = 0; i < completeQuads; i++)
                {
                    quadCounter++;
                    if (quadCounter > 4)
                    {
                        currentGroup++;
                        quadCounter = 1;
                    }

                    // Create a new quad
                    var quad = new Quad
                    {
                        Title = $"Quad {i + 1}",
                        TournamentId = tournament.Id,
                        QuadGroupNumber = currentGroup
                    };

                    _context.Quads.Add(quad);
                    await _context.SaveChangesAsync(); // Save to get the quad ID

                    // Assign 4 players to this quad
                    for (int j = 0; j < 4; j++)
                    {
                        int playerIndex = i * 4 + j;
                        if (playerIndex < tournamentPlayers.Count)
                        {
                            var player = tournamentPlayers[playerIndex];
                            player.QuadId = quad.Id;
                            player.QuadPosition = j + 1; // Position 1-4

                            // Add match information based on position
                            SetupPlayerMatchInfo(player);

                            _context.TournamentPlayers.Update(player);
                        }
                    }
                }

                await _context.SaveChangesAsync();
                Console.WriteLine($"Created {completeQuads} quads for tournament: {tournament.Name}");
            }
        }

        private void SetupPlayerMatchInfo(TournamentPlayer player)
        {
            // Set opponents and table numbers based on position
            switch (player.QuadPosition)
            {
                case 1:
                    player.Round1Opponent = "W v 4";
                    player.Round2Opponent = "B v 3";
                    player.Round3Opponent = "W v 2";
                    player.Round1Table = 1;
                    player.Round2Table = 1;
                    player.Round3Table = 1;
                    break;
                case 2:
                    player.Round1Opponent = "W v 3";
                    player.Round2Opponent = "B v 4";
                    player.Round3Opponent = "B v 1";
                    player.Round1Table = 2;
                    player.Round2Table = 2;
                    player.Round3Table = 1;
                    break;
                case 3:
                    player.Round1Opponent = "B v 2";
                    player.Round2Opponent = "W v 1";
                    player.Round3Opponent = "W v 4";
                    player.Round1Table = 2;
                    player.Round2Table = 1;
                    player.Round3Table = 2;
                    break;
                case 4:
                    player.Round1Opponent = "B v 1";
                    player.Round2Opponent = "W v 2";
                    player.Round3Opponent = "B v 3";
                    player.Round1Table = 1;
                    player.Round2Table = 2;
                    player.Round3Table = 2;
                    break;
            }

            // Add some random match scores (0, 0.5, or 1)
            var possibleScores = new[] { "0", "0.5", "1" };

            // Only set scores for some matches (to simulate in-progress tournament)
            if (_random.Next(100) < 70) // 70% chance of having a round 1 result
            {
                player.Round1Score = possibleScores[_random.Next(possibleScores.Length)];
            }

            if (_random.Next(100) < 40) // 40% chance of having a round 2 result
            {
                player.Round2Score = possibleScores[_random.Next(possibleScores.Length)];
            }

            if (_random.Next(100) < 20) // 20% chance of having a round 3 result
            {
                player.Round3Score = possibleScores[_random.Next(possibleScores.Length)];
            }
        }
    }
}
