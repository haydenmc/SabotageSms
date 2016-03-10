using Microsoft.Data.Entity;

namespace SabotageSms.Models {
    public class ApplicationDbContext : DbContext {
        public DbSet<Player> Players { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Message> Messages { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Player>()
                .HasIndex(p => p.PhoneNumber)
                .IsUnique(true);
            
            modelBuilder
                .Entity<Game>()
                .HasIndex(g => g.JoinCode)
                .IsUnique(true);
        }
    }
}