using Microsoft.Data.Entity;

namespace SabotageSms.Models.DbModels
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<DbPlayer> Players { get; set; }
        public DbSet<DbGame> Games { get; set; }
        public DbSet<DbMessage> Messages { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<DbPlayer>()
                .HasIndex(p => p.PhoneNumber)
                .IsUnique(true);
            
            modelBuilder
                .Entity<DbGame>()
                .HasIndex(g => g.JoinCode)
                .IsUnique(true);
        }
    }
}