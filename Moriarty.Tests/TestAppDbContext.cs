using Microsoft.EntityFrameworkCore;
using Moriarty.Web.Data.Models;

namespace Moriarty.Tests;

public class TestAppDbContext : DbContext
{
    public TestAppDbContext(DbContextOptions<TestAppDbContext> options) : base(options) { }

    public DbSet<Campaign> Campaigns { get; set; }
    public DbSet<Character> Characters { get; set; }

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
    }
}
