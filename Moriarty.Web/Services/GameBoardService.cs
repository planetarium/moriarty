using Moriarty.Web.Components.Pages;
using Moriarty.Web.Data.Models;

namespace Moriarty.Web.Services;

public class GameBoardService
{
    private readonly List<IGameBoard> _boards = [];

    public void Bind(IGameBoard board)
    {
        if (!_boards.Contains(board))
        {
            _boards.Add(board);
        }
    }

    public void Unbind(IGameBoard board)
    {
        if (_boards.Contains(board))
        {
            _boards.Remove(board);
        }
    }

    public void DisplaySuspect(Character suspect)
    {
        foreach (IGameBoard board in _boards)
        {
            board.DisplaySuspect(suspect);
        }
    }

    public void DisplayVictim(Character victim)
    {
        foreach (IGameBoard board in _boards)
        {
            board.DisplayVictim(victim);
        }
    }

    public void SetScene(string description)
    {
        foreach (IGameBoard board in _boards)
        {
            board.ChangeScene(description);
        }
    }

    public void EndSesison()
    {
        foreach (IGameBoard board in _boards)
        {
            board.EndSession();
        }
    }
}