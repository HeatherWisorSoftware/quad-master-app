using Microsoft.EntityFrameworkCore;
using QuadMasterApp.Data.Models;


// manages database connection and coordinates data operations.
// We could have other database connections here in the future
namespace QuadMasterApp.Data
{
    public class TournamentContext :DbContext
    {
        public TournamentContext(DbContextOptions<TournamentContext> options) : base(options)
        {
        }
        public DbSet<Player> Players { get; set; }
        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<TournamentPlayer> TournamentPlayers { get; set; }
        // DbSet for Quads
        public DbSet<Quad>Quads { get; set; }
        //override base method from DBContext 
        public DbSet<QuadMatch>QuadMatches{get; set;}



        //standard pattern for implementing a many-to-many relationship in Entity Framework Core
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Calls the parent class implementation first.
            base.OnModelCreating(modelBuilder);

 
            // Add unique constraint on TournamentId and PlayerId
            modelBuilder.Entity<TournamentPlayer>()
                .HasIndex(tp => new { tp.TournamentId, tp.PlayerId })
                .IsUnique();

            //One Tournament can have many Tournament Players
            modelBuilder.Entity<TournamentPlayer>()
               .HasOne(tp => tp.Tournament)
               .WithMany(t => t.TournamentPlayers)
               .HasForeignKey(tp => tp.TournamentId);

            // One Tournament can have many TournamentPlayers
            modelBuilder.Entity<TournamentPlayer>()
                .HasOne(tp => tp.Tournament)
                .WithMany(p => p.TournamentPlayers)
                .HasForeignKey(tp => tp.TournamentId);

            //quad configuration

            modelBuilder.Entity<TournamentPlayer>()
                .HasOne(tp => tp.Quad)
                .WithMany(q => q.Players)
                .HasForeignKey(tp => tp.QuadId)
                .IsRequired(false);


            modelBuilder.Entity<Quad>()
                .HasOne(q => q.Tournament)
                .WithMany(t => t.Quads)
                .HasForeignKey(q => q.TournamentId);

            modelBuilder.Entity<QuadMatch>()
                .HasOne(qm => qm.Quad)
                .WithMany()
                .HasForeignKey(qm => qm.QuadId);

            modelBuilder.Entity<QuadMatch>()
                .HasOne(qm => qm.PlayerOne)
                .WithMany()
                .HasForeignKey(qm => qm.PlayerOneId);

            modelBuilder.Entity<QuadMatch>()
                .HasOne(qm => qm.PlayerTwo)
                .WithMany()
                .HasForeignKey(qm => qm.PlayerTwoId);
        }
    }   
}
