namespace Genshin_Calculator.Core.Models;

public class LevelOptionRow
{
    public LevelOptionRow(Level single)
    {
        this.Left = single;
    }

    public LevelOptionRow(Level left, Level right)
    {
        this.Left = left;
        this.Right = right;
    }

    public Level? Left { get; }

    public Level? Right { get; }
}