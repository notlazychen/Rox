using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rox.Extensions.Tetris;

public enum GameStatus
{
    Wait,
    Running,
    Over,
}

public class GameBoard
{
    public readonly IList<BlockBase> Blocks = new List<BlockBase> {
            new BlockI(),
            new BlockJ(),
            new BlockL(),
            new BlockT(),
            new BlockZ(),
            new BlockS(),
            new BlockO(),
        };

    public GameStatus Status { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    /// <summary>
    /// 一维是行数Y，二维是列数X
    /// </summary>
    public Cell[,] Cells { get; private set; }

    public BlockBase Block { get; set; }

    public void Build(int width, int height)
    {
        Width = width;
        Height = height;
        Cells = new Cell[width, height];
        for (int x = 0; x < Cells.GetLength(0); x++)
        {
            for (int y = 0; y < Cells.GetLength(1); y++)
            {
                var cell = new Cell(x, y);
                Cells[x, y] = cell;
            }
        }
        Status = GameStatus.Running;
    }

    /// <summary>
    /// 擦除当前的方块
    /// </summary>
    public void ClearBlock()
    {
        if (this.Block != null)
        {
            foreach (var s in Block.Shape())
            {
                if (s.X >= 0 && s.Y >= 0 && s.X < this.Width && s.Y < this.Height)
                {
                    this.Cells[s.X, s.Y].IsFill = false;
                }
            }
        }
    }

    /// <summary>
    /// 渲染方块
    /// </summary>
    public void PlaceBlock()
    {
        foreach (var s in this.Block.Shape())
        {
            if (s.X >= 0 && s.Y >= 0 && s.X < this.Width && s.Y < this.Height)
            {
                this.Cells[s.X, s.Y].IsFill = true;
            }
        }
    }

    public string Print()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"------[{DateTime.UtcNow.ToString("HH:mm:ss")}]------");
        for (int y = 0; y < this.Cells.GetLength(1); y++)
        {
            sb.Append("|");
            for (int x = 0; x < this.Cells.GetLength(0); x++)
            {
                var cell = this.Cells[x, y];
                sb.AppendFormat("{0}|", cell.ToString());
            }
            sb.AppendLine();
        }
        sb.AppendLine("-----------------------------");
        return sb.ToString();
    }

    public GameStatus OneFrame()
    {
        if (Status != GameStatus.Running)
        {
            return Status;
        }
        var cells = this.Cells;
        //擦除当前的方块
        this.ClearBlock();

        //检查游戏结束
        bool isGameOver = false;
        for (int x = 0; x < cells.GetLength(0); x++)
        {
            if (cells[x, 0].IsFill)
            {
                isGameOver = true;
                break;
            }
        }
        if (isGameOver)
        {
            Status = GameStatus.Over;
            return Status;
        }

        //检查消除
        for (int y = 0; y < cells.GetLength(1); y++)
        {
            bool allfill = true;
            for (int x = 0; x < cells.GetLength(0); x++)
            {
                if (!cells[x, y].IsFill)
                {
                    allfill = false;
                    break;
                }
            }
            if (allfill)
            {
                //生成一行新的
                for (int x = 0; x < cells.GetLength(0); x++)
                {
                    var cell0 = cells[x, 0];
                    cell0.IsFill = false;
                    cell0.Color = ConsoleColor.White;
                }
                //上面的行全部下移
                for (int cury = y; cury > 0; cury--)
                {
                    for (int x = 0; x < cells.GetLength(0); x++)
                    {
                        var cucell = cells[x, cury];
                        var upcell = cells[x, cury - 1];
                        cucell.IsFill = upcell.IsFill;
                        cucell.Color = upcell.Color;
                    }
                }
            }
        }

        //方块碰撞检测，如果到底取消方块
        if (this.Block != null)
        {
            ///检查是否触底，触底则放弃方块
            //var maxY = State.Block.Shape().Where(s => s.X >= 0 && s.Y >= 0 && s.X < this.State.Width && s.Y < this.State.Height)
            //    .Max(x => x.Y);
            if (this.Block.Shape().Where(s => s.X >= 0 && s.Y >= 0 && s.X < this.Width && s.Y < this.Height)//.Where(s => s.Y == maxY)
                .Any(s => s.Y + 1 == this.Height || this.Cells[s.X, s.Y + 1].IsFill))
            {
                this.PlaceBlock();
                this.Block = null;
            }
        }
        //如果没有的话就生成一只新的
        if (this.Block == null)
        {
            var rand = new Random(DateTime.Now.Second);
            int i = rand.Next(0, this.Blocks.Count);
            this.Block = this.Blocks[i];
            this.Block.X = this.Width / 2;
            this.Block.Y = 0;
        }

        //方块往下走一
        this.Block.Y += 1;
        //渲染方块
        this.PlaceBlock();
        return Status;
    }

    public void Move(Direction direction)
    {
        if (this.Block != null)
        {
            this.ClearBlock();
            int x = this.Block.X;
            int y = this.Block.Y;
            int rotate = this.Block.Orientation;
            switch (direction)
            {
                case Direction.Up:
                    this.Block.Rotate();
                    break;
                case Direction.Right:
                    this.Block.X += 1;
                    break;
                case Direction.Down:
                    this.Block.Y += 1;
                    break;
                case Direction.Left:
                    this.Block.X -= 1;
                    break;
            }
            //边界判断
            bool touch = this.Block.Shape().Any(s => s.X < 0 || s.X >= this.Width || s.Y >= this.Height
                || (s.Y >= 0 && this.Cells[s.X, s.Y].IsFill));
            if (touch)
            {
                this.Block.X = x;
                this.Block.Y = y;
                this.Block.Orientation = rotate;
            }
            //渲染方块
            this.PlaceBlock();
        }
    }
}
