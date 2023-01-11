using Orleans;
using Orleans.Runtime;
using Rox.Extensions.Tetris;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Barbecue.GrainInterfaces;

public interface ITetrisGame : IGrainWithStringKey
{
    Task<StreamId> Start();

    Task Move(Direction direction);

    //Task PauseOrResume();

    //Task Close();
    //Task Restart();
}
