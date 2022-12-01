namespace GameOfLife;

public readonly struct CellRange
{
    public readonly bool IsEmpty = true;
    public readonly int Start;
    public readonly int End;

    public CellRange() { }
    public CellRange(int start, int end)
    {
        Start = start;
        End = end;
        IsEmpty = false;
    }

    public override string ToString() => IsEmpty ? "Empty" : $"{Start}..{End}";
}
