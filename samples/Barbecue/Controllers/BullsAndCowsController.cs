using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Rox.Extensions.BullsAndCows;
using System.Collections.Concurrent;

namespace Barbecue.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class BullsAndCowsController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IMemoryCache _memoryCache;

        public BullsAndCowsController(ILogger<WeatherForecastController> logger, IMemoryCache memoryCache)
        {
            _logger = logger;
            this._memoryCache = memoryCache;
        }

        //[HttpGet()]
        //public GameInfo GetGameInfo()
        //{
        //    return null;
        //}

        [HttpPost]
        public ActionResult<string> Start()
        {
            var game = new Game();
            _memoryCache.Set("Game_" + UserId, game, DateTimeOffset.Now.AddDays(1));
            var robot = new Robot();
            robot.Start(game.Options);
            var answer = robot.GetNextAnswer();
            _memoryCache.Set("Robot_" + UserId, robot, DateTimeOffset.Now.AddDays(1));
            return Ok(answer);
        }

        [HttpPost]
        public ActionResult<Result> Guess([FromBody] GuessRequest req)
        {
            var game = _memoryCache.Get<Game>("Game_" + UserId);
            var result = game.Guess(req.GuessNumber);
            return Ok(result);
        }

        [HttpPost]
        public ActionResult<string> AskRobot([FromBody] ReplyRobotRequest req)
        {
            var robot = _memoryCache.Get<Robot>("Robot_" + UserId);
            robot.Return(Result.Parse(req.Result));
            var answer = robot.GetNextAnswer();
            return Ok(answer);
        }

        private string UserId { get { return this.HttpContext.Connection.RemoteIpAddress + ""/* + ":" + this.HttpContext.Connection.RemotePort*/; } }
    }

    public class ReplyRobotRequest
    {
        public string Result { get; set; }
    }


    public class GuessRequest
    {
        public string GuessNumber { get; set; }
    }

    public class GameInfo
    {
        public int CurrentRound { get; set; }
        public List<RoundInfo> History { get; set; } = new List<RoundInfo>();

    }

    public class RoundInfo
    {
        public int Round { get; set; }
        public List<Chance> Chances { get; set; } = new List<Chance>();
    }

    public class Chance
    {
        public string Name { get; set; }
        public string GuessNum { get; set; }
        public string Result { get; set; }
    }
}