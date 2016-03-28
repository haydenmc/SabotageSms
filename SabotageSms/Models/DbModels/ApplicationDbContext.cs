using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;

namespace SabotageSms.Models.DbModels
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<DbPlayer> Players { get; set; }
        public DbSet<DbGame> Games { get; set; }
        public DbSet<DbRound> Rounds { get; set; }
        public DbSet<DbGamePlayer> GamePlayers { get; set; }
        public DbSet<DbMessage> Messages { get; set; }
        
        public ApplicationDbContext()
        { }
        
        public ApplicationDbContext(IServiceProvider serviceProvider, DbContextOptions<ApplicationDbContext> options)
            : base(serviceProvider, options)
        { }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Indexes for important lookup fields
            modelBuilder
                .Entity<DbPlayer>()
                .HasIndex(p => p.PhoneNumber)
                .IsUnique(true);
            modelBuilder
                .Entity<DbGame>()
                .HasIndex(g => g.JoinCode)
                .IsUnique(true);
                
            // Composite keys for relating players to games
            modelBuilder
                .Entity<DbGamePlayer>()
                .HasKey(p => new { p.PlayerId, p.GameId });
        }
    }
}