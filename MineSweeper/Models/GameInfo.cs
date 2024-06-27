namespace MineSweeper.Models;

public class GameInfo
{
    public Guid game_id;
    public int width;
    public int height;
    public int mines_count;
    public bool completed;
    public char[,] gameField;
    public char[,] displayedField;
    
}