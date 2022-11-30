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
    public bool[,] Cells { get; }
    /// <summary>
    /// Cells from the previous generation
    /// </summary>
    public bool[,] OldCells { get; }

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

        if (oldState != state)
        {
            if (oldState)
                Population--;
            else
                Population++;
        }

        Cells[x, y] = state;
    }
    /// <summary>
    /// Inverts the state of a cell and updates the population.
    /// </summary>
    public void InvertCell(int x, int y)
    {
        var oldState = Cells[x, y];

        if (oldState)
            Population--;
        else
            Population++;

        Cells[x, y] = !oldState;
    }

    /// <summary>
    /// Counts the number of living neighbors for a given cell
    /// </summary>
    private int LivingNeighbours(int x, int y, bool[,] cells)
    {
        var aliveCount = 0;
        var left = x > 0 ? x - 1 : XLimit;
        var right = x < XLimit ? x + 1 : 0;
        var top = y > 0 ? y - 1 : YLimit;
        var bottom = y < YLimit ? y + 1 : 0;

        // Check left
        if (cells[left, y])
            aliveCount++;
        // Check top left
        if (cells[left, top])
            aliveCount++;
        // Check bottom left
        if (cells[left, bottom])
            aliveCount++;
        // Check right
        if (cells[right, y])
            aliveCount++;
        // Check top right
        if (cells[right, top])
            aliveCount++;
        // Check bottom right
        if (cells[right, bottom])
            aliveCount++;
        // Check top
        if (cells[x, top])
            aliveCount++;
        // Check bottom
        if (cells[x, bottom])
            aliveCount++;

        return aliveCount;
    }

    /// <summary>
    /// Updates the living state of all cells
    /// </summary>
    public void Cycle()
    {
        Generation++;

        Array.Copy(Cells, OldCells, Width * Height);

        for (int x = 0; x <= XLimit; x++)
            for (int y = 0; y <= YLimit; y++)
                SetCell(x, y, LivingNeighbours(x, y, OldCells) switch
                {
                    < 2 or > 3 => false,
                    3 when !Cells[x, y] => true,
                    _ => OldCells[x, y]
                });
    }
}
