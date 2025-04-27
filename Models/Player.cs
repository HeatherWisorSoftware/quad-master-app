namespace QuadMasterApp.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get;set; }
        public int Ranking { get; set; } 
        public string Email { get; set; }

        //Other player properties
        public List<TournamentPlayer> TournamentPlayers { get; set; }
    }
}
