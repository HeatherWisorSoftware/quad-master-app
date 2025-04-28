using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuadTest.Data;
using QuadTest.Models;

namespace QuadTest.Pages.TournamentPage
{
    /// <summary>
    /// The Index page model for the Tournament page.
    /// </summary>
    public class TournamentIndexModel : PageModel
    {
        private readonly TournamentContext _context;

        /// <summary>
        /// Constructor that initializes the model with the database context.
        /// </summary>
        /// <param name="context">The database context for accessing tournament data.</param>
        public TournamentIndexModel(TournamentContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Property to hold the list of tournaments retrieved from the database.
        /// </summary>
        public IList<Tournament> Tournaments { get; set; }

        /// <summary>
        /// Handles GET requests for the Tournament Index page.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public async Task OnGetAsync()
        {
            // Check if the request is an AJAX request by inspecting the headers.
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            // If the request is AJAX, set the layout to null to avoid rendering the full layout.
            if (isAjax)
            {
                ViewData["layout"] = null;
            }

            // Retrieve the list of tournaments from the database asynchronously.
            Tournaments = await _context.Tournaments
                .Include(t => t.TournamentPlayers)
                    .ThenInclude(tp => tp.Player)
                .ToListAsync();
        }
    }
}