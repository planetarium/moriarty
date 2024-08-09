using Moriarty.Web.Data.Models;

namespace Moriarty.Web.Components.Pages;

public interface IGameBoard
{
    void DisplaySuspect(Character character);

    void DisplayVictim(Character character);

    void ChangeScene(string title, string background);

    void EndSession();
}