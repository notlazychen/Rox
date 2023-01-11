using Barbecue.Controllers;
using Barbecue.GrainInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;
using Rox.Extensions.Autolink;
using Rox.Extensions.BullsAndCows;

namespace Barbecue
{
    public class TetrisHub : Hub
    {
        private readonly IGrainFactory _grainFactory;
        private readonly IStreamProvider _streamProvider;
        private string UserId { get { return this.Context.ConnectionId; } }
        public TetrisHub(IGrainFactory grainFactory, IServiceProvider serviceProvider)
        {
            _grainFactory = grainFactory;
            _streamProvider = serviceProvider.GetRequiredServiceByKey<string, IStreamProvider>("game");
        }

        public override async Task OnConnectedAsync()
        {
            string userId = UserId;
            var client = this.Clients.Caller;
            var room = _grainFactory.GetGrain<ITetrisGame>(UserId);
            var stream = _streamProvider.GetStream<RoomMsg>("tetris", UserId);
            await room.Start();
            await stream.SubscribeAsync((data, token) =>
            {
                Console.WriteLine(data.Msg);
                return client.SendAsync("info", data.Msg);
            });
            await base.OnConnectedAsync();
        }

        public async Task Send(string message)
        {
            await Clients.All.SendAsync("recv", message);
        }

        public async Task Move(string direction)
        {
            var room = _grainFactory.GetGrain<ITetrisGame>(UserId);
            switch (direction)
            {
                case "u":
                    await room.Move(Rox.Extensions.Tetris.Direction.Up);
                    break;
                case "d":
                    await room.Move(Rox.Extensions.Tetris.Direction.Down);
                    break;
                case "l":
                    await room.Move(Rox.Extensions.Tetris.Direction.Left);
                    break;
                case "r":
                    await room.Move(Rox.Extensions.Tetris.Direction.Right);
                    break;
            }
        }
    }
}
