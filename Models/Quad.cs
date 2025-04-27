using System.ComponentModel.DataAnnotations;

namespace QuadMasterApp.Models
{
    public class Quad
    {

        public int Id { get; set; }

        [Required]
        [Display(Name = "Quad Title")]
        public string Title { get; set; }

        public int TournamentId { get; set; }
        public Tournament Tournament { get; set; }

        //for pagination (1-4 on page 1, 5-8 on page 2 etc)
        public int QuadGroupNumber { get; set; }

        //Navigation property for players in this quad
        public List<TournamentPlayer> Players { get; set; } = new List<TournamentPlayer>();
    }
}
