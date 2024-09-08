namespace GameOfLife;

public partial class BoardPanel : Panel
{
    public enum DragDirection : byte { Unknown, Vertical, Horizontal }

    const int defaultBoardSize = 100;

    private readonly SolidBrush aliveBrush, deadBrush;
    private readonly Pen gridPen;
    private Size oldSize;
    private Point dragLastPosition;
    private bool dragInitialState, dragging, dragLocked;
    private DragDirection dragLockDirection;

    public Board Board
    {
        get => _board;
        set
        {
            if (value is null)
                return;

            _board = value!;
            AdjustSize(true);
        }
    }
    private Board _board;

    public int CellSize { get; private set; }

    public bool ShowGrid
    {
        get => _showGrid;
        set
        {
            _showGrid = value;

            if (value)
                DrawGrid();
            else
                DrawBoard();
        }
    }
    private bool _showGrid = true;

    #region Colors
    public static Color DefaultAliveColor => Color.Black;
    public static Color DefaultDeadColor => Color.White;
    public static Color DefaultGridColor => Color.Gray;

    public Color AliveColor
    {
        get => aliveBrush.Color;
        set
        {
            if (value == Color.Empty)
                return;

            aliveBrush.Color = value;
            DrawBoard();
        }
    }


    public Color DeadColor
    {
        get => deadBrush.Color;
        set
        {
            if (value == Color.Empty)
                return;

            deadBrush.Color = value;
            DrawBoard();
        }
    }

    public Color GridColor
    {
        get => gridPen.Color;
        set
        {
            if (value == Color.Empty)
                return;

            gridPen.Color = value;

            if (ShowGrid)
                DrawGrid();
        }
    }
    #endregion

    public BoardPanel()
    {
        InitializeComponent();

        aliveBrush = new SolidBrush(DefaultAliveColor);
        deadBrush = new SolidBrush(DefaultDeadColor);
        gridPen = new Pen(DefaultGridColor);
        oldSize = Size;
        _board = new(defaultBoardSize, defaultBoardSize);
    }

    public void SetAllColors(Color alive, Color dead, Color grid, bool redraw = true)
    {
        aliveBrush.Color = alive;
        deadBrush.Color = dead;
        gridPen.Color = grid;

        if (redraw)
            DrawBoard();
    }
    public void ResetColors(bool redraw = true) => SetAllColors(DefaultAliveColor, DefaultDeadColor, DefaultGridColor, redraw);

    #region Coordinate mapping
    /// <summary>
    /// Maps a board pixel to a cell position.
    /// </summary>
    public Point MapPixelToCell(int x, int y) => new(x / CellSize, y / CellSize);
    /// <inheritdoc cref="MapPixelToCell(int, int)"/>
    public Point MapPixelToCell(Point pixel) => MapPixelToCell(pixel.X, pixel.Y);

    /// <summary>
    /// Maps a pixel region to a cell region.
    /// </summary>
    public Rectangle MapPixelRegionToCellRegion(Rectangle region)
    {
        var lowCell = MapPixelToCell(region.Left, region.Top);
        var highCell = MapPixelToCell(region.Right, region.Bottom);

        var width = highCell.X - lowCell.X;
        var height = highCell.Y - lowCell.Y;

        if (width == 0 && region.Width > 0)
            width = 1;
        if (height == 0 && region.Height > 0)
            height = 1;

        return new(lowCell.X, lowCell.Y, width, height);
    }
    #endregion

    #region Sizing
    public Size AdjustSize(bool erase)
    {
        var xChanged = oldSize.Width != Width;
        var yChanged = oldSize.Height != Height;

        if (xChanged ^ yChanged) // If only one dimension changed, base the cell size on that dimension
            CellSize = xChanged ? Width / Board.Width : Height / Board.Height;
        else
            UnbiasedResize();

        if (CellSize == 0)
            CellSize = 1;

        oldSize = Size = new(Board.Width * CellSize, Board.Height * CellSize);

        if (erase)
        {
            Erase();

            if (ShowGrid)
                DrawGrid();
        }
        else
            DrawBoard();

        return Size;

        void UnbiasedResize()
        {
            var widthBasedCellSize = (int)Math.Round(Width / (double)Board.Width);
            var heightBasedCellSize = (int)Math.Round(Height / (double)Board.Height);

            var widthBasedDelta = GetDelta(widthBasedCellSize);
            var heightBasedDelta = GetDelta(heightBasedCellSize);

            CellSize = widthBasedDelta < heightBasedDelta ? widthBasedCellSize : heightBasedCellSize;

            int GetDelta(int cellSize) => Math.Abs(Width - Board.Width * cellSize) + Math.Abs(Height - Board.Height * cellSize);
        }
    }
    #endregion

    public void Cycle()
    {
        using var g = CreateGraphics();
        void HandleChange(object? _, Point p) => UpdateCell(p.X, p.Y, g);

        Board.CycleCellChanged += HandleChange;
        Board.Cycle();
        Board.CycleCellChanged -= HandleChange;
    }

    #region Drawing
    /// <summary>
    /// Erases the board.
    /// </summary>
    private void Erase(Graphics? g = null)
    {
        var graphics = g ?? CreateGraphics();
        graphics.FillRectangle(deadBrush, new(Point.Empty, Size));

        if (g is null)
            graphics.Dispose();
    }

    /// <summary>
    /// Erases and fully draws the current board state.
    /// </summary>
    private void DrawBoard(Graphics? g = null)
    {
        var graphics = g ?? CreateGraphics();
        DrawBoardRegion(new(Point.Empty, Size), graphics);

        if (g is null)
            graphics.Dispose();
    }

    /// <summary>
    /// Draws a region of the board state using an existing <see cref="Graphics"/> instance.
    /// </summary>
    /// <param name="pixelRegion">Region to draw in pixels</param>
    private void DrawBoardRegion(Rectangle pixelRegion, Graphics g)
    {
        g.FillRectangle(deadBrush, pixelRegion);

        var cellRegion = MapPixelRegionToCellRegion(pixelRegion);

        if (ShowGrid)
            DrawGridRegion(cellRegion, g);

        for (int x = cellRegion.Left; x < cellRegion.Right && x < Board.Width; x++)
            for (int y = cellRegion.Top; y < cellRegion.Bottom && y < Board.Height; y++)
                if (Board[x, y])
                    DrawAliveCell(x, y, g);
    }

    private void DrawAliveCell(int x, int y, Graphics g) => DrawCellBase(x, y, g, aliveBrush);
    private void DrawDeadCell(int x, int y, Graphics g) => DrawCellBase(x, y, g, deadBrush);
    private void DrawCellBase(int x, int y, Graphics g, Brush brush)
    {
        x *= CellSize;
        y *= CellSize;

        var width = CellSize;
        var height = CellSize;

        if (ShowGrid) // Draw the cell one pixel thiner in every direction to not cover the gird.
        {
            x++;
            y++;
            width--;
            height--;
        }

        g.FillRectangle(brush, x, y, width, height);
    }
    private void InvertCell(int x, int y)
    {
        using var g = CreateGraphics();

        Board.InvertCell(x, y);
        UpdateCell(Math.Clamp(x, 0, Board.Width - 1), Math.Clamp(y, 0, Board.Height - 1), g);
    }
    private void UpdateCell(int x, int y, Graphics g)
    {
        if (Board[x, y])
            DrawAliveCell(x, y, g);
        else
            DrawDeadCell(x, y, g);
    }

    /// <summary>
    /// Draws the full grid.
    /// </summary>
    private void DrawGrid()
    {
        using var g = CreateGraphics();
        DrawGridRegion(new(Point.Empty, Board.Size), g);
    }
    /// <summary>
    /// Draws the grid in a region of the panel.
    /// </summary>
    /// <param name="cellRegion">Region to draw in cells</param>
    private void DrawGridRegion(Rectangle cellRegion, Graphics g)
    {
        for (int x = cellRegion.Left; x <= cellRegion.Right; x += 2)
            g.DrawRectangle(gridPen, x * CellSize, 0, CellSize, cellRegion.Bottom * CellSize);
        for (int y = cellRegion.Top; y <= cellRegion.Bottom; y += 2)
            g.DrawRectangle(gridPen, 0, y * CellSize, cellRegion.Right * CellSize, CellSize);
    }
    #endregion

    #region EventHandlers
    protected override void OnPaint(PaintEventArgs pe) => DrawBoardRegion(pe.ClipRectangle, pe.Graphics);
    protected override void OnMouseDown(MouseEventArgs e)
    {
        dragging = true;

        var cell = MapPixelToCell(e.Location);
        using var g = CreateGraphics();

        dragInitialState = Board[cell.X, cell.Y];
        InvertCell(cell.X, cell.Y);
    }
    protected override void OnMouseMove(MouseEventArgs e)
    {
        if (!dragging)
            return;

        var cell = MapPixelToCell(e.Location);


        if (cell.X < 0 || cell.X > Board.Width - 1 || cell.Y < 0 || cell.Y > Board.Height - 1)
            return;

        if (cell == dragLastPosition || Board[cell.X, cell.Y] != dragInitialState)
            return;

        if (dragLocked)
        {
            switch (dragLockDirection)
            {
                case DragDirection.Unknown:
                    if (cell.X == dragLastPosition.X)
                        dragLockDirection = DragDirection.Vertical;
                    else if (cell.Y == dragLastPosition.Y)
                        dragLockDirection = DragDirection.Horizontal;
                    break;
                case DragDirection.Horizontal:
                    cell = new(cell.X, dragLastPosition.Y);
                    break;
                case DragDirection.Vertical:
                    cell = new(dragLastPosition.X, cell.Y);
                    break;
            }
        }

        dragLastPosition = cell;
        InvertCell(cell.X, cell.Y);
    }
    protected override void OnMouseUp(MouseEventArgs e) => dragging = false;

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.KeyCode == Keys.LShiftKey)
            dragLocked = true;
    }
    protected override void OnKeyUp(KeyEventArgs e)
    {
        if (e.KeyCode == Keys.LShiftKey)
        {
            dragLocked = false;
            dragLockDirection = DragDirection.Unknown;
        }
    }
    #endregion

    ~BoardPanel()
    {
        aliveBrush.Dispose();
        deadBrush.Dispose();
        gridPen.Dispose();
    }
}
