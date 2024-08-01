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
    private readonly GameBoardService _gameBoardService;
    private readonly ILogger<CampaignPlugin> _logger;

    public CampaignPlugin(
        AppDbContext dbContext,
        GameBoardService gameBoardService,
        ILoggerFactory loggerFactory)
    {
        _dbContext = dbContext;
        _gameBoardService = gameBoardService;
        _logger = loggerFactory.CreateLogger<CampaignPlugin>();
    }

    [KernelFunction(name: "set_scene")]
    [Description("Generate a prompt for DALL-e, to illustrate the scene where current conversation is taking")]
    public void SetScene(string description)
    {
        _gameBoardService.SetScene(description);
    }

    [KernelFunction("display_suspect_card")]
    [Description("Display a character card of suspect.")]
    public async Task DisplaySuspect(Guid campaignId, string name)
    {
        _logger.LogInformation("DisplaySuspect() called. {campaignId}, {name}", campaignId, name);
        Campaign campaign = await _dbContext.Campaigns
            .Include(c => c.Suspects)
            .FirstAsync(c => c.Id == campaignId);
        _gameBoardService.DisplaySuspect(campaign.Suspects.First(c => c.Name == name));
    }

    [KernelFunction("display_victim_card")]
    [Description("Display a character card of victim.")]
    public async Task DisplayVictim(Guid campaignId)
    {
        _logger.LogInformation("DisplayVictim() called. {campaignId}", campaignId);
        Campaign campaign = await _dbContext.Campaigns
            .Include(c => c.Victim)
            .FirstAsync(c => c.Id == campaignId);
        _gameBoardService.DisplayVictim(campaign.Victim);
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

    [KernelFunction("end_session")]
    [Description("Ends the current session and deactive all interface when end of game.")]
    public void EndSession()
    {
        _gameBoardService.EndSesison();
    }
}
