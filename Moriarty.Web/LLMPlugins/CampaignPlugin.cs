using System.ComponentModel;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using Moriarty.Web.Data;
using Moriarty.Web.Data.Models;
using Moriarty.Web.Services;

namespace Moriarty.Web.LLMPlugins;

public class CampaignPlugin(AppDbContext dbContext, GameBoardService gameBoardService)
{
    [KernelFunction(name: "set_scene")]
    [Description("Generate a prompt for DALL-e, to illustrate the scene where current conversation is taking")]
    public void SetScene(string description)
    {
        gameBoardService.SetScene(description);
    }

    [KernelFunction("display_suspect")]
    [Description("Display a suspect information to the game board.")]
    public async Task DisplaySuspect(string name)
    {
        Character character = await dbContext.Characters.FirstAsync(c => c.Name == name);
        if (character is not null)
        {
            gameBoardService.DisplaySuspect(character);
        }
    }

    [KernelFunction("display_victim")]
    [Description("Display a victim information to the game board.")]
    public async Task DisplayVictim(string name)
    {
        Character character = await dbContext.Characters.FirstAsync(c => c.Name == name);
        if (character is not null)
        {
            gameBoardService.DisplayVictim(character);
        }
    }

    [KernelFunction("get_campaign")]
    [Description("Gets the campaign corresponding to the given ID as JSON representation.")]
    [return: Description("The JSON encoded represation of the campaign.")]
    public async Task<string> GetCampaign(string id)
    {
        Campaign campaign = await dbContext.Campaigns
                .Include(c => c.Victim)
                .Include(c => c.Offender)
                .Include(c => c.Suspects)
                .FirstAsync(c => c.Id == new Guid(id));
        return JsonSerializer.Serialize(campaign);
    }
}
