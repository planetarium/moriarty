using Moriarty.Web.Data.Models;

namespace Moriarty.Web.Components.Pages;

public interface IGameBoard
{
    void OnSuspectDisplayed(Character character);

    void OnVictimDisplayed(Character character);

    void OnSceneChanged(string background);
    void OnSessionEnded();
}