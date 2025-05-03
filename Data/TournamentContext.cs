using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WebUi.Server.Data.Models;


// manages database connection and coordinates data operations.
// We could have other database connections here in the future
namespace WebUi.Server.Data
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



        //standard pattern for implementing a many-to-many relationship in Entity Framework Core
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Calls the parent class implementation first.
            base.OnModelCreating(modelBuilder);

            //Defines a composite primary key for the TournamentPlayer table, consisting of both TournamentId and PlayerId columns.
            modelBuilder.Entity<TournamentPlayer>()
                .HasKey(tp => new { tp.TournamentId, tp.PlayerId });
            
            //Each TournamentPlayer belongs to one Tournament and each Tournament can have many TournamentPlayer entries
            modelBuilder.Entity<TournamentPlayer>()
               .HasOne(tp => tp.Tournament)
               .WithMany(t => t.TournamentPlayers)
               .HasForeignKey(tp => tp.TournamentId);

            //Each TournamentPlayer belongs to one Player and each Player can have many TournamentPlayer entries
            modelBuilder.Entity<TournamentPlayer>()
                .HasOne(tp => tp.Player)
                .WithMany(p => p.TournamentPlayers)
                .HasForeignKey(tp => tp.PlayerId);

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
        }
    }   
}
