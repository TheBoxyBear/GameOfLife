using static System.Windows.Forms.AxHost;

namespace GameOfLife;

public class Board
{
    private readonly int XLimit, YLimit;

    public event EventHandler<Point>? CycleCellChanged;

    /// <summary>
    /// Dimensions of the board in cell count
    /// </summary>
    public Size Size { get; }

    /// <summary>
    /// Width of the board
    /// </summary>
    public int Width => Size.Width;

    /// <summary>
    /// Height of the board
    /// </summary>
    public int Height => Size.Height;

    public ulong Generation { get; private set; }

    public uint Population { get; private set; }

    /// <summary>
    /// State of cells where <see langword="true"/> is a living cell
    /// </summary>
    private bool[,] Cells { get; }

    /// <summary>
    /// Cells from the previous generation
    /// </summary>
    private  bool[,] OldCells { get; }

    public bool this[int x, int y]
    {
        get => Cells[x, y];
        set
        {
            ref bool cell = ref Cells[x, y];

            if (cell != value)
                UpdatePopulation(value);

            Cells[x, y] = value;
        }
    }

    #region Optimization
    private bool cycling = false;
    private bool[,] searchZone, oldSearchZone;
    private CellRange[] rowSearchRange, oldRowSearchRange;
    #endregion

    /// <summary>
    /// Creates a new instance using a set size.
    /// </summary>
    public Board(Size size)
    {
        Size = size;
        XLimit = Width - 1;
        YLimit = Height - 1;

        Cells = new bool[size.Width, size.Height];
        OldCells = new bool[size.Width, size.Height];
        searchZone = new bool[size.Width, size.Height];
        oldSearchZone = new bool[size.Width, size.Height];

        Array.Fill(rowSearchRange = new CellRange[Height], new());
        Array.Fill(oldRowSearchRange = new CellRange[Height], new());
    }

    /// <summary>
    /// Creates a new instance using a set width and height.
    /// </summary>
    public Board(int width, int height) : this(new(width, height)) { }

    private void SetCellUnsafe(int x, int y, bool state)
    {
        Cells[x, y] = state;

        UpdatePopulation(state);
        UpdateSearchZone(x, y);

        if (cycling)
            CycleCellChanged?.Invoke(this, new(x, y));
    }

    private void UpdatePopulation(bool newState)
        => Population += (uint)(newState ? 1 : -1);

    /// <summary>
    /// Inverts the state of a cell and updates the population.
    /// </summary>
    public void InvertCell(int x, int y)
    {
        this[x, y] = !this[x, y];
        searchZone[x, y] = true;
        UpdateSearchZone(x, y);
    }

    private void UpdateSearchZone(int x, int y)
    {
        var left   = WrapIndex(x - 1, XLimit);
        var right  = WrapIndex(x + 1, XLimit);
        var top    = WrapIndex(y - 1, YLimit);
        var bottom = WrapIndex(y + 1, YLimit);

        searchZone[left, y]       = true;
        searchZone[right, y]      = true;
        searchZone[x, top]        = true;
        searchZone[x, bottom]     = true;

        searchZone[left, top]     = true;
        searchZone[left, bottom]  = true;
        searchZone[right, top]    = true;
        searchZone[right, bottom] = true;

        UpdatePresence(y);
        UpdatePresence(WrapIndex(top, YLimit));
        UpdatePresence(WrapIndex(bottom, YLimit));

        void UpdatePresence(int index)
        {
            var presence = rowSearchRange[index];

            presence = presence.IsEmpty
                ? new(x, x)
                : new(x - 1 < presence.Start ? x - 1 : presence.Start,x + 1 > presence.End ? x + 1 : presence.End);

            rowSearchRange[index] = presence;
        }
    }

    /// <summary>
    /// Updates the living state of all cells
    /// </summary>
    public void Cycle()
    {
        Generation++;

        cycling = true;

        Array.Copy(searchZone, oldSearchZone, Width * Height);
        Array.Copy(rowSearchRange, oldRowSearchRange, Height);
        Array.Copy(Cells, OldCells, Width * Height);
        Array.Fill(rowSearchRange = new CellRange[Height], new());

        searchZone = new bool[Width, Height];

        unchecked
        {
            for (int y = 0; y < YLimit; y++)
                CheckRow(y);
        }

        cycling = false;

        void CheckSetCell(int x, int y)
        {
            if (!oldSearchZone[x, y])
                return;

            var n = LivingNeighbors(x, y);

            switch (n)
            {
                case < 2 or > 3:
                    if (OldCells[x, y])
                        SetCellUnsafe(x, y, false);
                    break;
                case 3 when !OldCells[x, y]:
                    SetCellUnsafe(x, y, true);
                    break;
            }
        }

        void CheckRow(int y)
        {
            unchecked
            {
                var range = oldRowSearchRange[y];

                if (range.IsEmpty)
                    return;

                var limit = SetLimits(range, XLimit, x => CheckSetCell(x, y));

                for (int x = limit.Start; x <= limit.End; x++)
                    CheckSetCell(x, y);
            }
        }

        int LivingNeighbors(int x, int y)
        {
            unchecked
            {
                var aliveCount = 0;
                var left = WrapIndex(x - 1, XLimit);
                var right = WrapIndex(x + 1, XLimit);
                var top = WrapIndex(y - 1, YLimit);
                var bottom = WrapIndex(y + 1, YLimit);

                if (OldCells[left, y])
                    aliveCount++;
                if (OldCells[left, top])
                    aliveCount++;
                if (OldCells[left, bottom])
                    aliveCount++;
                if (OldCells[right, y])
                    aliveCount++;
                if (OldCells[right, top])
                    aliveCount++;
                if (OldCells[right, bottom])
                    aliveCount++;
                if (OldCells[x, top])
                    aliveCount++;
                if (OldCells[x, bottom])
                    aliveCount++;

                return aliveCount;
            }
        }

        CellRange SetLimits(CellRange presence, int limit, Action<int> preliminaryCheck)
        {
            int start = presence.Start, end = presence.End;

            if (start == -1)
            {
                preliminaryCheck(limit);
                start = 0;

                if (end == limit)
                {
                    end = limit - 1;
                    return new(start, end);
                }
            }
            if (end == limit + 1)
            {
                preliminaryCheck(0);

                if (start == 0)
                    start = 1;

                end = limit;
            }

            return new(start, end);
        }
    }

    static int WrapIndex(int index, int limit) => index > limit ? 0 : index < 0 ? limit : index;
}
