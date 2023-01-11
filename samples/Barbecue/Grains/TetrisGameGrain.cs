using Barbecue.GrainInterfaces;
using Barbecue;
using Orleans;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orleans.Runtime;
using Rox.Extensions.Tetris;

namespace Barbecue.Grains;

public class TetrisGameGrain : Grain, ITetrisGame
{
    private IAsyncStream<RoomMsg> _stream;
    private GameBoard _game;
    private IDisposable _timer;
    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        var streamProvider = this.GetStreamProvider("game");
        var streamId = StreamId.Create("tetris", this.GetPrimaryKeyString());
        _stream = streamProvider.GetStream<RoomMsg>(streamId);
        return base.OnActivateAsync(cancellationToken);
    }

    public Task<StreamId> Start()
    {
        _game = new GameBoard();
        _game.Build(14, 24);
        _timer = this.RegisterTimer(OnFrameAsync, null, TimeSpan.FromSeconds(0), TimeSpan.FromMilliseconds(500));
        return Task.FromResult(_stream.StreamId);
    }

    private async Task OnFrameAsync(object state)
    {
        var status = _game.OneFrame();
        if (status == GameStatus.Over)
        {
            await _stream.OnNextAsync(new RoomMsg("gm", "Game Over!"));
            _timer.Dispose();
            return;
        }
        //通知前端当前帧状态
        var sb = this._game.Print();
        await _stream.OnNextAsync(new RoomMsg("frame", $"[{this.GetPrimaryKeyString()}]\n{sb}"));
    }

    public async Task Move(Direction direction)
    {
        //渲染方块
        _game.Move(direction);
        var sb = this._game.Print();
        await _stream.OnNextAsync(new RoomMsg("frame", $"[{this.GetPrimaryKeyString()}]\n{sb}"));            
    }

    //public Task PauseOrResume()
    //{
    //    throw new NotImplementedException();
    //}

    //public Task Close()
    //{
    //    throw new NotImplementedException();
    //}

    //public Task Restart()
    //{
    //    throw new NotImplementedException();
    //}
}
