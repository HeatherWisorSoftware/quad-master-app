using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuadMasterApp.Data;
using QuadMasterApp.Models;
using QuadTest.Data;
using QuadTest.Models;

namespace QuadTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MainController : ControllerBase
    {
        private readonly TournamentContext _context;
        private readonly ILogger<MainController> _logger;

        public MainController(TournamentContext context, ILogger<MainController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Get dashboard data
        [HttpGet("dashboard")]
        public async Task<ActionResult<object>> GetDashboardData()
        {
            try
            {
                // Check if there are any tournaments
                var hasTournaments = await _context.Tournaments.AnyAsync();

                // Get the 5 latest tournaments
                var latestTournaments = await _context.Tournaments
                    .OrderByDescending(t => t.Id)
                    .Take(5)
                    .Select(t => new
                    {
                        t.Id,
                        t.Name,
                        t.Date,
                        t.VenueName,
                        PlayerCount = _context.TournamentPlayers.Count(tp => tp.TournamentId == t.Id)
                    })
                    .ToListAsync();

                return Ok(new
                {
                    HasTournaments = hasTournaments,
                    LatestTournaments = latestTournaments
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving dashboard data");
                return StatusCode(500, new { error = "An error occurred while retrieving dashboard data." });
            }
        }

        // Create a test tournament
        [HttpPost("test-database")]
        public async Task<ActionResult<object>> TestDatabase()
        {
            try
            {
                var testTournament = new Tournament
                {
                    Name = "Test Tournament " + DateTime.Now.ToString(),
                    Date = DateTime.Now,
                    VenueName = "Test Venue",
                    Street = "123 Test St",
                    City = "Test City",
                    State = "TS",
                    PostalCode = "12345"
                };

                _context.Tournaments.Add(testTournament);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = $"Successfully created tournament with ID: {testTournament.Id}",
                    tournamentId = testTournament.Id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing database");
                return StatusCode(500, new
                {
                    success = false,
                    error = $"Error: {ex.Message}"
                });
            }
        }

        // Get all tournaments
        [HttpGet("tournaments")]
        public async Task<ActionResult<IEnumerable<object>>> GetTournaments()
        {
            try
            {
                var tournaments = await _context.Tournaments
                    .OrderByDescending(t => t.Date)
                    .Select(t => new
                    {
                        t.Id,
                        t.Name,
                        t.Date,
                        t.VenueName,
                        t.City,
                        t.State,
                        t.Street,
                        t.PostalCode,
                        PlayerCount = _context.TournamentPlayers.Count(tp => tp.TournamentId == t.Id)
                    })
                    .ToListAsync();

                return Ok(tournaments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tournaments");
                return StatusCode(500, new { error = "An error occurred while retrieving tournaments." });
            }
        }

        // Get a specific tournament
        [HttpGet("tournaments/{id}")]
        public async Task<ActionResult<object>> GetTournament(int id)
        {
            try
            {
                var tournament = await _context.Tournaments
                    .Where(t => t.Id == id)
                    .Select(t => new
                    {
                        t.Id,
                        t.Name,
                        t.Date,
                        t.VenueName,
                        t.City,
                        t.State,
                        t.Street,
                        t.PostalCode,
                        Players = _context.TournamentPlayers
                            .Where(tp => tp.TournamentId == id)
                            .Select(tp => new
                            {
                                tp.Player.Id,
                                tp.Player.Name,
                                tp.Player.Rating,
                                QuadId = tp.Player.QuadId,
                                QuadName = tp.Player.Quad.Name
                            })
                            .ToList()
                    })
                    .FirstOrDefaultAsync();

                if (tournament == null)
                {
                    return NotFound(new { error = "Tournament not found" });
                }

                return Ok(tournament);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tournament with ID {Id}", id);
                return StatusCode(500, new { error = "An error occurred while retrieving the tournament." });
            }
        }

        // Create a tournament
        [HttpPost("tournaments")]
        public async Task<ActionResult<object>> CreateTournament([FromBody] Tournament tournament)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _context.Tournaments.Add(tournament);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Tournament created successfully",
                    tournamentId = tournament.Id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating tournament");
                return StatusCode(500, new
                {
                    success = false,
                    error = "An error occurred while creating the tournament."
                });
            }
        }

        // Get all players
        [HttpGet("players")]
        public async Task<ActionResult<IEnumerable<object>>> GetPlayers()
        {
            try
            {
                var players = await _context.Players
                    .OrderByDescending(p => p.Rating)
                    .Select(p => new
                    {
                        p.Id,
                        p.Name,
                        p.Rating,
                        TournamentId = p.TournamentPlayer != null ? p.TournamentPlayer.TournamentId : (int?)null,
                        TournamentName = p.TournamentPlayer != null ? p.TournamentPlayer.Tournament.Name : null,
                        QuadId = p.QuadId,
                        QuadName = p.Quad != null ? p.Quad.Name : null
                    })
                    .ToListAsync();

                return Ok(players);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving players");
                return StatusCode(500, new { error = "An error occurred while retrieving players." });
            }
        }

        // Get all quads
        [HttpGet("quads")]
        public async Task<ActionResult<IEnumerable<object>>> GetQuads()
        {
            try
            {
                var quads = await _context.Quads
                    .Select(q => new
                    {
                        q.Id,
                        q.Name,
                        q.TournamentId,
                        TournamentName = q.Tournament.Name,
                        Players = q.Players
                            .OrderByDescending(p => p.Rating)
                            .Select(p => new
                            {
                                p.Id,
                                p.Name,
                                p.Rating
                            })
                            .ToList()
                    })
                    .ToListAsync();

                return Ok(quads);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving quads");
                return StatusCode(500, new { error = "An error occurred while retrieving quads." });
            }
        }

        // Additional endpoints can be added as needed for your application
    }
}