using Microsoft.AspNetCore.Mvc;
using MineSweeper.Models;
using MineSweeper.Repositories;

namespace MineSweeper;

public class GameService
{
    private GamesRepository _gamesRepository;

    public GameService(GamesRepository gamesRepository)
    {
        _gamesRepository = gamesRepository;
    }

    public async Task<GameInfoResponse> StartNewGame(NewGameRequest gameRequest)
    {
        GameInfo game = new()
        {
            game_id = Guid.NewGuid(),
            width = gameRequest.width,
            height = gameRequest.height,
            mines_count = gameRequest.mines_count,
            
        };
        (game.gameField, game.displayedField) = CreateField(game.width, gameRequest.height, gameRequest.mines_count);

        _gamesRepository.GamesList.Add(game);

        return new GameInfoResponse
        {
            game_id = game.game_id.ToString(),
            width = game.width,
            height = game.height,
            mines_count = game.mines_count,
            completed = false,
            field = game.displayedField
        };
    }

    public async Task<GameInfoResponse> PlayerTurn(GameTurnRequest gameRequest)
    {
        var curGame = _gamesRepository.GamesList.FirstOrDefault(x => x.game_id.ToString().Equals(gameRequest.game_id));
        var x = curGame.gameField;
        if (curGame.gameField[gameRequest.row, gameRequest.col] == 'M')
        {
            for (int i = 0; i < curGame.width; i++)
            {
                for (int j = 0; j < curGame.height; j++)
                {
                    if (curGame.gameField[i, j].Equals('M'))
                        curGame.gameField[i, j] = 'X';
                }
            }

            curGame.completed = true;
            return new GameInfoResponse
            {
                game_id = curGame.game_id.ToString(),
                width = curGame.width,
                height = curGame.height,
                mines_count = curGame.mines_count,
                completed = curGame.completed,
                field = curGame.gameField
            };
        }
    
        RevealCell(gameRequest.row, gameRequest.col, curGame);
        if (CheckWin(curGame))
        {
            curGame.completed = true;
            return new GameInfoResponse
            {
                game_id = curGame.game_id.ToString(),
                width = curGame.width,
                height = curGame.height,
                mines_count = curGame.mines_count,
                completed = curGame.completed,
                field = curGame.gameField
            };
        }

        return new GameInfoResponse
        {
            game_id = curGame.game_id.ToString(),
            width = curGame.width,
            height = curGame.height,
            mines_count = curGame.mines_count,
            completed = false,
            field = curGame.displayedField
        };
    }
    
    private void RevealCell(int row, int col, GameInfo curGame)
    {
        if (row < 0 || row >= curGame.height || col < 0 || col >= curGame.width || curGame.displayedField[row, col] != ' ')
        {
            return;
        }
    
        curGame.displayedField[row, col] = curGame.gameField[row, col];
    
        if (curGame.gameField[row, col] == '0')
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    RevealCell(row + x, col + y, curGame);
                }
            }
        }
    }
    
    private bool CheckWin(GameInfo curGame)
    {
        for (int i = 0; i < curGame.height; i++)
        {
            for (int j = 0; j < curGame.width; j++)
            {
                if (curGame.gameField[i, j] != 'M' && curGame.displayedField[i, j] == ' ')
                {
                    return false;
                }
            }
        }
    
        return true;
    }

    private (char[,], char[,]) CreateField(int width, int height, int mines_count)
    {
        var gameField = new char[width, height];
        var responseField = new char[width, height];
        for (int i = 0; i < height; i++)
        for (int j = 0; j < width; j++)
        {
            gameField[i, j] = ' ';
            responseField[i, j] = ' ';
        }

        PlaceMines(width, height, mines_count, gameField);
        CalculateMines(width, height,gameField);
        return (gameField, responseField);
    }

    private void PlaceMines(int width, int height, int mines_count, char[,] gameField)
    {
        Random rand = new Random();
        int placedMines = 0;
        while (placedMines < mines_count)
        {
            int row = rand.Next(height);
            int column = rand.Next(width);
            if (gameField[row, column] != 'M')
            {
                gameField[row, column] = 'M';
                placedMines++;
            }
        }
    }

    private void CalculateMines(int width, int height, char[,] gameField)
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (gameField[i, j] != 'M')
                {
                    int mineCount = 0;
                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            int newRow = i + x;
                            int newCol = j + y;
                            if (newRow >= 0 && newRow < height && newCol >= 0 && newCol < width &&
                                gameField[newRow, newCol] == 'M')
                            {
                                mineCount++;
                            }
                        }
                    }

                    gameField[i, j] = mineCount.ToString()[0];
                }
            }
        }
    }
}