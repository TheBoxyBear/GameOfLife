namespace GameOfLife;

public class Board
{
    private readonly int XLimit, YLimit;

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
    public bool[,] Cells { get; private set; }
    /// <summary>
    /// Cells from the previous generation
    /// </summary>
    public bool[,] OldCells { get; private set; }

    #region Optimization
    private CellRange[] oldColumnPresences;
    private CellRange[] columnPresences;
    private CellRange rowPresence;
    private readonly int[] rowAliveCounts;
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

        oldColumnPresences = CreatePresences(); ;
        columnPresences = CreatePresences();
        rowAliveCounts = new int[Height];

        CellRange[] CreatePresences()
        {
            var presences = new CellRange[Width];

            for (int i = 0; i < presences.Length; i++)
                presences[i] = new();

            return presences;
        }
    }

    /// <summary>
    /// Creates a new instance using a set width and height.
    /// </summary>
    public Board(int width, int height) : this(new(width, height)) { }

    /// <summary>
    /// Sets the state of a cell and updates the population.
    /// </summary>
    public void SetCell(int x, int y, bool state)
    {
        var oldState = Cells[x, y];

        if (oldState == state)
            return;

        SetCellUnsafe(x, y, state);
    }
    private void SetCellUnsafe(int x, int y, bool state)
    {
        Cells[x, y] = state;
        UpdatePopulation(x, y, state);
    }
    /// <summary>
    /// Inverts the state of a cell and updates the population.
    /// </summary>
    public void InvertCell(int x, int y) => UpdatePopulation(x, y, Cells[x, y] = !Cells[x, y]);

    /// <summary>
    /// Updates the population and counts after a cell change.
    /// </summary>
    private void UpdatePopulation(int x, int y, bool newState)
    {
        var updateColumnPresence = true;
        var updateRowPresence = true;

        if (newState)
        {
            Population++;
            rowAliveCounts[y]++;

            if (rowPresence.IsEmpty)
            {
                rowPresence = new(y, y);
                updateRowPresence = false;
            }

            if (columnPresences[x].IsEmpty)
            {
                columnPresences[x] = new(y, y);
                updateColumnPresence = false;
            }
        }
        else
        {
            Population--;

            if (rowPresence.IsEmpty)
                updateRowPresence = false;
            else if (--rowAliveCounts[y] == 0 && rowPresence.Start == rowPresence.End)
            {
                rowPresence = new();
                updateRowPresence = false;
            }

            if (columnPresences[x].IsEmpty)
                updateColumnPresence = false;
            else if (columnPresences[x].Start == columnPresences[x].End)
            {
                columnPresences[x] = new();
                updateColumnPresence = false;
            }
        }

        if (updateColumnPresence)
            UpdatePresence(ref columnPresences[x], y, y => Cells[x, y]);
        if (updateRowPresence)
            UpdatePresence(ref rowPresence, y, y => rowAliveCounts[y] > 0);

        static void UpdatePresence(ref CellRange presence, int position, Func<int, bool> isLimit)
        {
            var checkStart = presence.Start;
            var checkEnd = presence.End;

            if (position < presence.Start)
                checkStart = position;
            else if (position > presence.End)
                checkEnd = position;
            else
                return;

            for (int i = checkStart; i <= checkEnd; i++)
                if (isLimit(i))
                {
                    checkStart = i;
                    break;

                }
            for (int i = checkEnd; i >= checkStart; i--)
                if (isLimit(i))
                {
                    checkEnd = i;
                    break;
                }

            presence = new(checkStart, checkEnd);
        }
    }

    /// <summary>
    /// Updates the living state of all cells
    /// </summary>
    public void Cycle()
    {
        Generation++;

        if (rowPresence.IsEmpty)
            return;

        var oldRowPresence = rowPresence;
        Array.Copy(Cells, OldCells, Width * Height);
        Array.Copy(columnPresences, oldColumnPresences, oldColumnPresences.Length);

        //var xCheck = SetLimits(rowPresence, XLimit, update);

        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                CheckSetCell(x, y);

        //for (int x = xCheck.Start; x <= xCheck.End; x++)
        //    CheckColumn(x, update);

        void CheckSetCell(int x, int y)
        {
            var n = LivingNeighbours(x, y);

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
        void CheckColumn(int x)
        {
            for (int y = 0; y < Width; y++)
                CheckSetCell(x, y);
        }
        int LivingNeighbours(int x, int y)
        {
            var aliveCount = 0;
            var left = x > 0 ? x - 1 : XLimit;
            var right = x < XLimit ? x + 1 : 0;
            var top = y > 0 ? y - 1 : YLimit;
            var bottom = y < YLimit ? y + 1 : 0;

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
        CellRange SetLimits(CellRange presence, int limit)
        {
            var checkStart = presence.Start - 1;
            var checkEnd = presence.End + 1;

            if (checkStart == -1)
            {
                CheckColumn(limit);
                checkStart = 0;

                if (checkEnd == limit)
                {
                    checkEnd = XLimit - 1;
                    return new(checkStart, checkEnd);
                }
            }
            if (checkEnd == limit + 1)
            {
                CheckColumn(0);

                if (checkStart == 0)
                    checkStart = 1;

                checkEnd = limit;
            }

            return new(checkStart, checkEnd);
        }
    }
}
