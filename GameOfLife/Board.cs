﻿namespace GameOfLife;

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
    public bool[,] Cells { get; private set; }
    /// <summary>
    /// Cells from the previous generation
    /// </summary>
    public bool[,] OldCells { get; private set; }

    private bool[,] searchZone, oldSearchZone;

    #region Optimization
    private bool cycling = false;
    private CellRange rowPresence = new(), columnPresence = new();
    private readonly int[] rowAliveCounts, columnAliveCounts;
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

        rowAliveCounts = new int[Height];
        columnAliveCounts = new int[Width];
    }

    /// <summary>
    /// Creates a new instance using a set width and height.
    /// </summary>
    public Board(int width, int height) : this(new(width, height)) { }

    private void SetCellUnsafe(int x, int y, bool state)
    {
        Cells[x, y] = state;
        //UpdatePopulation(x, y, state);
        UpdateSearchZone(x, y);

        //if (cycling)
        //    CycleCellChanged?.Invoke(this, new(x, y));
    }
    /// <summary>
    /// Inverts the state of a cell and updates the population.
    /// </summary>
    public void InvertCell(int x, int y)
    {
        Cells[x, y] = !Cells[x, y];
        //UpdatePopulation(x, y, Cells[x, y] = !Cells[x, y]);
        UpdateSearchZone(x, y);
    }

    /// <summary>
    /// Updates the population and counts after a cell change.
    /// </summary>
    private void UpdatePopulation(int x, int y, bool newState)
    {
        bool updateRowPresence, updateColumnPresence;

        if (newState)
        {
            Population++;

            updateColumnPresence = PreUpdate(x, ref columnPresence, columnAliveCounts);
            updateRowPresence = PreUpdate(y, ref rowPresence, rowAliveCounts);

            bool PreUpdate(int position, ref CellRange presence, int[] aliveCounts)
            {
                aliveCounts[position]++;

                if (!presence.IsEmpty)
                    return true;

                presence = new(position, position);
                return false;
            }
        }
        else
        {
            Population--;

            updateColumnPresence = PreUpdate(x, ref columnPresence, columnAliveCounts);
            updateRowPresence = PreUpdate(y, ref rowPresence, rowAliveCounts);

            bool PreUpdate(int position, ref CellRange presence, int[] aliveCounts)
            {
                if (presence.IsEmpty)
                    return false;

                if (--aliveCounts[position] != 0 || presence.Length != 0)
                    return true;

                presence = new();
                return false;
            }
        }

        if (updateColumnPresence)
            UpdatePresence(x, ref columnPresence, columnAliveCounts);
        if (updateRowPresence)
            UpdatePresence(y, ref rowPresence, rowAliveCounts);

        static void UpdatePresence(int position, ref CellRange presence, int[] aliveCounts)
        {
            var checkStart = presence.Start;
            var checkEnd = presence.End;

            if (position < presence.Start)
                checkStart = position;
            else if (position > presence.End)
                checkEnd = position;
            else
                return;

            unchecked
            {
                // Find start
                for (int i = checkStart; i <= checkEnd; i++)
                    if (aliveCounts[i] > 0)
                    {
                        checkStart = i;
                        break;

                    }
                // Find end
                for (int i = checkEnd; i >= checkStart; i--)
                    if (aliveCounts[i] > 0)
                    {
                        checkEnd = i;
                        break;
                    }
            }

            presence = new(checkStart, checkEnd);
        }
    }
    private void UpdateSearchZone(int x, int y)
    {
        var left = x > 0 ? x - 1 : XLimit;
        var right = x < XLimit ? x + 1 : 0;
        var top = y > 0 ? y - 1 : YLimit;
        var bottom = y < YLimit ? y + 1 : 0;

        searchZone[x, y] = true;

        searchZone[left, y] = true;
        searchZone[right, y] = true;
        searchZone[x, top] = true;
        searchZone[x, bottom] = true;

        searchZone[left, top] = true;
        searchZone[left, bottom] = true;
        searchZone[right, top] = true;
        searchZone[left, bottom] = true;
    }

    /// <summary>
    /// Updates the living state of all cells
    /// </summary>
    public void Cycle()
    {
        Generation++;


        //if (rowPresence.IsEmpty)
        //    return;

        cycling = true;

        Array.Copy(searchZone, oldSearchZone, Width * Height);
        Array.Copy(Cells, OldCells, Width * Height);

        searchZone = new bool[Width, Height];

        //var yCheck = SetLimits(rowPresence, YLimit, CheckRow);

        //unchecked
        //{
        //    for (int y = yCheck.Start; y <= yCheck.End; y++)
        //        CheckRow(y);
        //}

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
        void CheckRow(int y)
        {
            //unchecked
            //{
            //    if (rowAliveCounts[y] == 0
            //    && columnAliveCounts[WarpIndex(y - 1, YLimit)] == 0
            //    && columnAliveCounts[WarpIndex(y + 1, YLimit)] == 0)
            //        return;

            //    var xCheck = SetLimits(columnPresence, XLimit, x => CheckSetCell(x, y));

            //    for (int x = xCheck.Start; x <= xCheck.End; x++)
            //        CheckSetCell(x, y);
            //}

            unchecked
            {
                for (int x = 0; x <= XLimit; x++)
                    CheckSetCell(x, y);
            }
        }
        int LivingNeighbours(int x, int y)
        {
            unchecked
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
        }
        CellRange SetLimits(CellRange presence, int limit, Action<int> preliminaryCheck)
        {
            var checkStart = presence.Start - 1;
            var checkEnd = presence.End + 1;

            if (checkStart == -1)
            {
                preliminaryCheck(limit);
                checkStart = 0;

                if (checkEnd == limit)
                {
                    checkEnd = limit - 1;
                    return new(checkStart, checkEnd);
                }
            }
            if (checkEnd == limit + 1)
            {
                preliminaryCheck(0);

                if (checkStart == 0)
                    checkStart = 1;

                checkEnd = limit;
            }

            return new(checkStart, checkEnd);
        }
        int WarpIndex(int index, int limit) => index > limit ? 0 : index == -1 ? limit : index;
    }
}
