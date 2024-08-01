using Microsoft.EntityFrameworkCore;
using Moriarty.Web.Data.Models;

namespace Moriarty.Web.Data;

public class AppDbContext : DbContext
{
    public DbSet<Campaign> Campaigns { get; set; }
    
    public DbSet<Character> Characters { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=/Users/longfin/moriarty/Moriarty.Web/campaigns.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Campaign>()
            .HasMany(c => c.Suspects)
            .WithMany(c => c.SuspectedInCampaigns)
            .UsingEntity<Dictionary<string, object>>(
                "CampaignSuspect",
                j => j
                    .HasOne<Character>()
                    .WithMany()
                    .HasForeignKey("CharacterId")
                    .HasConstraintName("FK_CampaignSuspect_Character_CharacterId")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j
                    .HasOne<Campaign>()
                    .WithMany()
                    .HasForeignKey("CampaignId")
                    .HasConstraintName("FK_CampaignSuspect_Campaign_CampaignId")
                    .OnDelete(DeleteBehavior.Cascade));
        
        modelBuilder.Entity<Campaign>()
            .HasOne(c => c.Victim)
            .WithMany()
            .HasForeignKey(c => c.VictimId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Campaign>()
            .HasOne(c => c.Offender)
            .WithMany()
            .HasForeignKey(c => c.OffenderId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Add this part to configure the one-to-many relationship between Campaign and Clue
        modelBuilder.Entity<Campaign>()
            .HasMany(c => c.Clues)
            .WithOne(c => c.Campaign)
            .HasForeignKey(c => c.CampaignId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}