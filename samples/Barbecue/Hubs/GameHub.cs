﻿using Barbecue.Controllers;
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
    public class GameHub : Hub
    {
        private readonly IGrainFactory _grainFactory;
        private readonly IStreamProvider _streamProvider;
        private string UserId { get { return this.Context.ConnectionId; } }
        public GameHub(IGrainFactory grainFactory, IServiceProvider serviceProvider)
        {
            _grainFactory = grainFactory;
            _streamProvider = serviceProvider.GetRequiredServiceByKey<string, IStreamProvider>("barbecue");
        }

        public override async Task OnConnectedAsync()
        {
            string userId = UserId;
            var client = this.Clients.Caller;
            var room = _grainFactory.GetGrain<IRoomGrain>(UserId);
            var stream = _streamProvider.GetStream<RoomMsg>("room", UserId);
            await stream.SubscribeAsync((data, token) =>
            {
                return client.SendAsync("info", data.Msg);
            });
            await base.OnConnectedAsync();
        }

        public async Task Send(string message)
        {
            await Clients.All.SendAsync("recv", message);
        }

        public async Task Start()
        {
            var room = _grainFactory.GetGrain<IRoomGrain>(UserId);
            await room.StartGame();
        }

        public async Task Guess(string guessNumber)
        {
            var room = _grainFactory.GetGrain<IRoomGrain>(UserId);
            var result = await room.Guess(guessNumber);
        }

        public async Task StartRobot()
        {
            var room = _grainFactory.GetGrain<IRoomGrain>(UserId);
            await room.StartRobot();
        }

        public async Task AskRobot(string result)
        {
            var room = _grainFactory.GetGrain<IRoomGrain>(UserId);
            await room.AskRobot(result);
        }
    }
}
