using Barbecue.Controllers;
using Barbecue.GrainInterfaces;
using Microsoft.AspNetCore.Mvc;
using Orleans.Runtime;
using Orleans.Streams;
using Rox.Extensions.BullsAndCows;
using System.IO;

namespace Barbecue.Grains;

public class BacRoomGrain : Grain, IBacRoomGrain
{
    Game game = new Game();
    Robot robot = new Robot();
    private IAsyncStream<RoomMsg> _stream = null!;

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        var streamProvider = this.GetStreamProvider("game");
        var streamId = StreamId.Create("bac", this.GetPrimaryKeyString());
        _stream = streamProvider.GetStream<RoomMsg>(streamId);
        return base.OnActivateAsync(cancellationToken);
    }

    public async ValueTask StartGame()
    {
        game = new Game();
        await _stream.OnNextAsync(new RoomMsg("info", "开始新局"));
    }

    public async ValueTask<string> Guess(string guessNumber)
    {
        var result = game.Guess(guessNumber);
        await _stream.OnNextAsync(new RoomMsg("info", $"[{game.Round}]{guessNumber}->{result.A}A{result.B}B"));
        return result.ToString();
    }

    public async ValueTask<string> StartRobot()
    {
        robot = new Robot(game.Options);
        var answer = robot.GetNextAnswer();
        await _stream.OnNextAsync(new RoomMsg("info", $"[{robot.Round}]{answer}"));
        return answer;
    }

    public async ValueTask<string> AskRobot(string result)
    {
        robot.Return(Result.Parse(result));
        var answer = robot.GetNextAnswer();
        await _stream.OnNextAsync(new RoomMsg("info", $"[{robot.Round}]{answer}"));
        return answer;
    }
}
