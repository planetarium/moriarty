using Moriarty.Web.Data.Models;

namespace Moriarty.Web;

public interface IGameBoard
{
    void OnSuspectDisplayed(Character character);

    void OnVictimDisplayed(Character character);

    void OnSceneChanged(string background);
}