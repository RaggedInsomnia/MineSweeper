using System.Net;
using System.Text.Json;
using System.Web.Http.Cors;
using Microsoft.AspNetCore.Mvc;
using MineSweeper.Models;
using MineSweeper.Repositories;
using Newtonsoft.Json;

namespace MineSweeper.Controllers;


[ApiController]

[Route("[controller]")]
public class GameController : ControllerBase
{
    private readonly GameService _gameService;
    private readonly GamesRepository _gamesRepository;

    public GameController(GameService gameService, GamesRepository gamesRepository)
    {
        _gameService = gameService;
        _gamesRepository = gamesRepository;
    }
    
    
    [HttpPost("new")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    [Consumes("application/json")]
    public async Task<ActionResult<string>> CreateNewGame([FromBody] NewGameRequest args)
    {
        if(args.height > 30 || args.height <= 0 || args.width > 30 || args.width <= 0)
             return BadRequest("Ошибка! Размеры поля должны быть от 0 до 30");
        if (args.mines_count <= 0 || args.mines_count > args.width * args.height - 1)
            return BadRequest("Ошибка! Некорректное количество мин");
        var response = await _gameService.StartNewGame(args);
        return Ok(JsonConvert.SerializeObject(response));
    }

    [HttpPost("turn")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    [Consumes("application/json")]
    public async Task<ActionResult<string>> PlayerTurn([FromBody] GameTurnRequest args)
    {
        var curGame = _gamesRepository.GamesList.FirstOrDefault(x => x.game_id.ToString().Equals(args.game_id)); 
        if (curGame == null)
            return BadRequest(JsonConvert.SerializeObject("Игра не была создана или была удалена!"));
        if (curGame.completed)
            return BadRequest(JsonConvert.SerializeObject("Игра уже закончена!"));
        if(curGame.displayedField[args.row, args.col] != ' ')
            return BadRequest(JsonConvert.SerializeObject("Клетка уже открыта!"));
        var response = await _gameService.PlayerTurn(args);
        return Ok(JsonConvert.SerializeObject(response));
    }
}