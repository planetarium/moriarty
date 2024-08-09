using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using Moriarty.Web.Data;
using Moriarty.Web.Data.Models;
using Moriarty.Web.Services;

namespace Moriarty.Web.LLMPlugins;

public class GameBoardPlugin
{
    private readonly AppDbContext _dbContext;
    private readonly GameBoardService _gameBoardService;
    private readonly ILogger<GameBoardPlugin> _logger;

    public GameBoardPlugin(GameBoardService gameBoardService, AppDbContext appDbContext, ILoggerFactory loggerFactory)
    {
        _gameBoardService = gameBoardService;
        _dbContext = appDbContext;
        _logger = loggerFactory.CreateLogger<GameBoardPlugin>();
    }
    
    [KernelFunction(name: "set_scene")]
    [Description("Set a description for current scene.")]
    public void SetScene(string title, string description)
    {
        _gameBoardService.SetScene(title, description);
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

    [KernelFunction("end_session")]
    [System.ComponentModel.Description("Ends the current session and deactive all interface when end of game.")]
    public void EndSession()
    {
        _gameBoardService.EndSesison();
    }
}