﻿using Rox.Extensions.BullsAndCows;

namespace Barbecue.GrainInterfaces;

public interface IBacRoomGrain : IGrainWithStringKey
{
    ValueTask StartGame();

    ValueTask<string> Guess(string guessNumber);

    ValueTask<string> StartRobot();
    ValueTask<string> AskRobot(string result);
}
