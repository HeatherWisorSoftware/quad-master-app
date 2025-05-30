using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace QuadMasterApp.Data.Models
{
    public enum TournamentStatus
    {
        Registration,
        Active,
        Completed,
        Cancelled
    }

    public class Tournament
    {
        public int Id{ get; set; }

        //[Required(ErrorMessage = "Tournament name is required")]
        //[StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters")]
        [Display(Name = "Tournament Name")]
        public string Name { get; set; }

        //[Required(ErrorMessage = "Start date is required")]
        [Display(Name = "Date")]
        public DateTime? Date { get; set; }

        // Address Information
        [Display(Name = "Venue Name")]
        public string VenueName { get; set; }

        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string? Country { get; set; }
        public string? FormattedAddress { get; set; }

        [Display(Name = "Tournament Active")]
        public bool IsLocked { get; set; } = false;

        public DateTime? CompletedDate { get; set; }

        public List<TournamentPlayer> TournamentPlayers { get; set; }

        public List<Quad> Quads { get; set; } = new List<Quad>();
    }



}
