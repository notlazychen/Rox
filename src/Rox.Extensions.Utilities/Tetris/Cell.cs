namespace Rox.Extensions.Tetris;

public class Cell
{
    public ConsoleColor Color { get; set; }
    public bool IsFill { get; set; } = false;

    public int X { get; }
    public int Y { get; }

    public Cell(int x, int y)
    {
        X = x;
        Y = y;
    }

    public override string ToString()
    {
        return this.IsFill ? "+" : " ";
    }
}
