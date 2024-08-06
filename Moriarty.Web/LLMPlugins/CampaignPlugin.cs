using System.ComponentModel;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using Moriarty.Web.Data;
using Moriarty.Web.Data.Models;
using Moriarty.Web.Services;

namespace Moriarty.Web.LLMPlugins;

public class CampaignPlugin
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<CampaignPlugin> _logger;

    public CampaignPlugin(
        AppDbContext dbContext,
        ILoggerFactory loggerFactory)
    {
        _dbContext = dbContext;
        _logger = loggerFactory.CreateLogger<CampaignPlugin>();
    }

    [KernelFunction("get_campaign")]
    [Description("Gets the campaign corresponding to the given ID as JSON representation.")]
    [return: Description("The JSON encoded represation of the campaign.")]
    public async Task<string> GetCampaign(string id)
    {
        _logger.LogInformation("GetCampign() called. {id}", id);
        Campaign campaign = await _dbContext.Campaigns
                .Include(c => c.Victim)
                .Include(c => c.Offender)
                .Include(c => c.Suspects)
                .Include(c=> c.Clues)
                .FirstAsync(c => c.Id == new Guid(id));
        return JsonSerializer.Serialize(campaign);
    }
}
