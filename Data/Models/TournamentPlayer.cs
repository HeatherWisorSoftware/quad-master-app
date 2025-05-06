using QuadMasterApp.Data.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuadMasterApp.Data.Models
{
    public class TournamentPlayer
    {
        //primary key
        public int Id { get; set; }

        //join entity Player (foreign key)
        public int PlayerId { get; set; }
        public Player Player { get; set; }

        //join entity Tournament (foreign key)
        public int TournamentId { get; set; }
        public Tournament Tournament { get; set; }

        //Quad Assignment
        public int? QuadId { get; set; }
        public Quad Quad { get; set; }

        //Helper property to check if player is assigned to a quad
        public bool IsAssigned => QuadId.HasValue;

        // Quad position (1-4, with 1 typically being highest rated)
        public int? QuadPosition { get; set; }

        // Scores for each round
        [NotMapped]
        public string Round1Score { get; set; }

        [NotMapped]
        public string Round2Score { get; set; }

        [NotMapped]
        public string Round3Score { get; set; }

        // Opponent info
        [NotMapped]
        public string Round1Opponent { get; set; } // Format: "W v 4" or "B v 2" 
        
        [NotMapped]
        public string Round2Opponent { get; set; }

        [NotMapped]
        public string Round3Opponent { get; set; }

        // Table numbers
        [NotMapped]
        public int? Round1Table { get; set; }

        [NotMapped]
        public int? Round2Table { get; set; }

        [NotMapped]
        public int? Round3Table { get; set; }

        // Total score calculation
        [NotMapped]
        public decimal TotalScore
        {
            get
            {
                decimal total = 0;
                if (decimal.TryParse(Round1Score, out decimal r1)) total += r1;
                if (decimal.TryParse(Round2Score, out decimal r2)) total += r2;
                if (decimal.TryParse(Round3Score, out decimal r3)) total += r3;
                return total;
            }
        }

        [NotMapped] 
        public List<string> Scores { get; set; } = [];
    }
}