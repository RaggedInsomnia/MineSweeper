using Microsoft.AspNetCore.Mvc;

namespace MineSweeper.Models;

[BindProperties]
public class NewGameRequest
{

    public int width
    {
        get;
        set;
    }

    public int height
    {
        get;
        set;
    }

    public int mines_count
    {
        get;
        set;
    }
}