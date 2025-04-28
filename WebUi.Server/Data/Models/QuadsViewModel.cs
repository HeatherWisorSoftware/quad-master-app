namespace WebUi.Server.Data.Models
{
    public class QuadsViewModel
    {
        public int TournamentId { get; set; }
        public string TournamentName { get; set; } = string.Empty;  // Add this line
        public List<Quad> Quads { get; set; } = new List<Quad>();
        public List<Quad> AllQuads { get; set; } = new List<Quad>(); // All quads for the dropdown
        public List<TournamentPlayer> UnassignedPlayers { get; set; } = new List<TournamentPlayer>();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public bool AllowQuadGeneration { get; set; } = true;
        public bool AllowPlayerRemoval { get; set; } = true;
        public bool AllowPlayerAssignment { get; set; } = true;
        public bool AreAllQuadsFull { get; set; } = false;
    }
}