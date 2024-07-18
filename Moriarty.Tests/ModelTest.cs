using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Moriarty.Web.Data;
using Moriarty.Web.Data.Models;

namespace Moriarty.Tests;

public class ModelTest
{
    private DbContextOptions<TestAppDbContext> GetInMemoryOptions()
    {
        return new DbContextOptionsBuilder<TestAppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public void CanAddAndRetrieveCampaign()
    {
        var options = GetInMemoryOptions();

        using var ctx1 = new TestAppDbContext(options);
        var character1 = new Character
        {
            Id = Guid.NewGuid(),
            Name = "Victim",
            Description = "Victim Description",
            ProfilePicture = new byte[0]
        };

        var character2 = new Character
        {
            Id = Guid.NewGuid(),
            Name = "Offender",
            Description = "Offender Description",
            ProfilePicture = new byte[0]
        };

        ctx1.Characters.AddRange(character1, character2);

        var campaign = new Campaign
        {
            Id = Guid.NewGuid(),
            Title = "Test Campaign",
            Plot = "Test Plot",
            Victim = character1,
            Offender = character2,
        };

        ctx1.Campaigns.Add(campaign);
        ctx1.SaveChanges();

        using TestAppDbContext ctx2 = new(options);
        var returned = ctx2.Campaigns.Include(c => c.Victim).Include(c => c.Offender).FirstOrDefault();
        Assert.NotNull(returned);
        Assert.Equal("Test Campaign", returned.Title);
        Assert.Equal("Victim", returned.Victim.Name);
        Assert.Equal("Offender", returned.Offender.Name);
    }

    [Fact]
    public void CanAddAndRetrieveCampaignWithSuspects()
    {
        var options = GetInMemoryOptions();

        using (var context = new TestAppDbContext(options))
        {
            var victim = new Character
            {
                Id = Guid.NewGuid(),
                Name = "Victim",
                Description = "Victim Description",
                ProfilePicture = new byte[0]
            };

            var offender = new Character
            {
                Id = Guid.NewGuid(),
                Name = "Offender",
                Description = "Offender Description",
                ProfilePicture = new byte[0]
            };

            var suspect1 = new Character
            {
                Id = Guid.NewGuid(),
                Name = "Suspect1",
                Description = "Suspect Description",
                ProfilePicture = new byte[0]
            };

            var suspect2 = new Character
            {
                Id = Guid.NewGuid(),
                Name = "Suspect2",
                Description = "Suspect Description",
                ProfilePicture = new byte[0]
            };

            context.Characters.AddRange(victim, offender, suspect1, suspect2);

            var campaign = new Campaign
            {
                Id = Guid.NewGuid(),
                Title = "Test Campaign",
                Plot = "Test Plot",
                Victim = victim,
                Offender = offender,
                Suspects = new List<Character> { suspect1, suspect2 }
            };

            context.Campaigns.Add(campaign);
            context.SaveChanges();
        }

        using (var context = new TestAppDbContext(options))
        {
            var campaign = context.Campaigns.Include(c => c.Suspects).FirstOrDefault();
            Assert.NotNull(campaign);
            Assert.Equal(2, campaign.Suspects.Count);
        }
    }
}