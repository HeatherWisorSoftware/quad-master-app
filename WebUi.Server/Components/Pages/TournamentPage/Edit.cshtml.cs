using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using QuadTest.Data;
using QuadTest.Models;

namespace QuadTest.Pages.TournamentPage
{
    public class EditModel : PageModel
    {
        //This line declares a private, read-only field named _context of type
        //TournamentContext from the QuadTest.Data namespace.
        private readonly QuadTest.Data.TournamentContext _context;

        //Constructor class called EditModel. Accepts parameter Tournament Context.
        public EditModel(QuadTest.Data.TournamentContext context)
        {
            //Inside the constructor, the passed context parameter is assigned
            //to the private field _context._context field can be used throughout
            //class to interact with database
            _context = context;
        }
        //used to bind form data from HTTP requests to properties.
        //Declares property named Tournament of type Tournament.
        //BindProperty tells framework to bind form values to this property
        //when form is submitted
        [BindProperty]
        public Tournament Tournament { get; set; } = default!;

        //list of integers. Collected multiple selected values like check boxes
        //or multi select
        [BindProperty]
        public List<int> SelectedPlayerIds { get; set; } = new List<int>();

        //SelectList, which is a class in ASP.NET designed specifically for
        //populating dropdown lists and multi-select controls in views
        public SelectList PlayerList { get; set; }

        //holds collection of Player objects
        //It could be used to display the current players
        //in the tournament before any changes are made
        public List<Player> CurrentPlayers { get; set; } = new List<Player>();

        // handles HTTP Get requests to load and prepare data for a tournament
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            //If no ID is provided, returns a NotFound result (404)
            if (id == null)
            {
                return NotFound();
            }

            // Load tournament with all properties
            var tournament = await _context.Tournaments
                .Include(t => t.TournamentPlayers)
                .ThenInclude(tp => tp.Player)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (tournament == null)
            {
                return NotFound();
            }

            Tournament = tournament;

            // Get current player IDs for selection
            SelectedPlayerIds = tournament.TournamentPlayers
                .Select(tp => tp.PlayerId)
                .ToList();

            // Get current players for display
            CurrentPlayers = tournament.TournamentPlayers
                .Select(tp => tp.Player)
                .ToList();

            // Populate the player dropdown
            PlayerList = new SelectList(
                await _context.Players
                .Select(p => new
                {
                    Id = p.Id,
                    FullName = $"{p.FirstName} {p.LastName}"
                }).ToListAsync(),
                "Id", "FullName");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Remove validation for these fields
            ModelState.Remove("Tournament.TournamentPlayers");
            ModelState.Remove("Tournament.Country");
            ModelState.Remove("Tournament.FormattedAddress");

            // Handle missing fields by setting default values
            if (string.IsNullOrEmpty(Tournament.Country))
            {
                Tournament.Country = "USA"; // Default country
            }

            if (string.IsNullOrEmpty(Tournament.FormattedAddress))
            {
                Tournament.FormattedAddress = ""; // Empty string
            }

            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState is invalid:");
                foreach (var state in ModelState)
                {
                    if (state.Value.Errors.Count > 0)
                    {
                        Console.WriteLine($"- {state.Key}: {string.Join(", ", state.Value.Errors.Select(e => e.ErrorMessage))}");
                    }
                }

                // Reload form data
                await LoadFormDataAsync();
                return Page();
            }

            try
            {
                // Load existing tournament
                var tournamentToUpdate = await _context.Tournaments
                    .Include(t => t.TournamentPlayers)
                    .FirstOrDefaultAsync(t => t.Id == Tournament.Id);

                if (tournamentToUpdate == null)
                {
                    return NotFound();
                }

                // Update tournament properties
                tournamentToUpdate.Name = Tournament.Name;
                tournamentToUpdate.Date = Tournament.Date;

                tournamentToUpdate.VenueName = Tournament.VenueName;
                tournamentToUpdate.Street = Tournament.Street;
                tournamentToUpdate.City = Tournament.City;
                tournamentToUpdate.State = Tournament.State;
                tournamentToUpdate.PostalCode = Tournament.PostalCode;
                tournamentToUpdate.Country = Tournament.Country;
                tournamentToUpdate.FormattedAddress = Tournament.FormattedAddress;

                // Get existing tournament players
                var existingPlayers = tournamentToUpdate.TournamentPlayers.ToList();

                // Clear existing players
                tournamentToUpdate.TournamentPlayers.Clear();
                await _context.SaveChangesAsync();

                // Add selected players
                if (SelectedPlayerIds != null && SelectedPlayerIds.Any())
                {
                    foreach (var playerId in SelectedPlayerIds)
                    {
                        tournamentToUpdate.TournamentPlayers.Add(new TournamentPlayer
                        {
                            TournamentId = Tournament.Id,
                            PlayerId = playerId
                        });
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TournamentExists(Tournament.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task LoadFormDataAsync()
        {
            // Reload player dropdown
            PlayerList = new SelectList(
                await _context.Players
                .Select(p => new
                {
                    Id = p.Id,
                    FullName = $"{p.FirstName} {p.LastName}"
                }).ToListAsync(),
                "Id", "FullName");

            // Reload current players
            CurrentPlayers = await _context.TournamentPlayers
                .Where(tp => tp.TournamentId == Tournament.Id)
                .Include(tp => tp.Player)
                .Select(tp => tp.Player)
                .ToListAsync();
        }

        private bool TournamentExists(int id)
        {
            return _context.Tournaments.Any(e => e.Id == id);
        }
    }
}