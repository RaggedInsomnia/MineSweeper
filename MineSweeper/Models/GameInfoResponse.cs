namespace MineSweeper.Models;

public class GameInfoResponse
{
    public string game_id;
    public int width;
    public int height;
    public int mines_count;
    public bool completed;
    public char[,] field;

}