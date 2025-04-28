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
    public class DeleteModel : PageModel
    {
        private readonly QuadTest.Data.TournamentContext _context;

        public DeleteModel(QuadTest.Data.TournamentContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Tournament Tournament { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tournament = await _context.Tournaments.FirstOrDefaultAsync(m => m.Id == id);

            if (tournament is not null)
            {
                Tournament = tournament;

                return Page();
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tournament = await _context.Tournaments.FindAsync(id);
            if (tournament != null)
            {
                Tournament = tournament;
                _context.Tournaments.Remove(Tournament);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
