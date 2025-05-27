namespace QuadMasterApp.Data.Models
{
    public class QuadMatch
    {
        public int Id { get; set; }

        // Foreign keys for relationships
        public int QuadId { get; set; }
        public Quad Quad { get; set; }

        public int PlayerOneId { get; set; }
        public TournamentPlayer PlayerOne { get; set; }

        public int PlayerTwoId { get; set; }
        public TournamentPlayer PlayerTwo { get; set; }

        // Match details
        public int RoundNumber { get; set; }  // 1, 2, or 3
        public int TableNumber { get; set; }  // Typically 1 or 2

        // Preserve player info even if they're moved later
        public string PlayerOneName { get; set; }
        public string PlayerTwoName { get; set; }

        public int PlayerOneRanking { get; set; }
        public int PlayerTwoRanking { get; set; }

        // Results
        public string PlayerOneColor { get; set; }  // "W" or "B"
        public decimal? PlayerOneScore { get; set; }  // Typically 0, 0.5, or 1
        public decimal? PlayerTwoScore { get; set; }  // Typically 0, 0.5, or 1

        public bool IsComplete { get; set; } = false;

        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
    }
}
