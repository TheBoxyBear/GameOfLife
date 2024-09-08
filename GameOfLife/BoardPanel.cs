using unvell.D2DLib;
using unvell.D2DLib.WinForm;

namespace GameOfLife;

public partial class BoardPanel : D2DControl
{
    public enum DragDirection : byte { Unknown, Vertical, Horizontal }

    const int defaultBoardSize = 100;

    private readonly D2DSolidColorBrush aliveBrush, deadBrush;
    private readonly D2DPen gridPen;
    private D2DGraphics? graphics;

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
            AdjustSize();
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

            Render(value ? DrawGrid : DrawBoard);
        }
    }
    private bool _showGrid = true;

    #region Colors
    public static Color DefaultAliveColor => Color.Black;
    public static Color DefaultDeadColor => Color.White;
    public static Color DefaultGridColor => Color.Gray;

    public Color AliveColor
    {
        get => D2DColor.ToGDIColor(aliveBrush.Color);
        set
        {
            if (value == Color.Empty)
                return;

            aliveBrush.Color = D2DColor.FromGDIColor(value);
            Render(DrawBoard);
        }
    }


    public Color DeadColor
    {
        get => D2DColor.ToGDIColor(deadBrush.Color);
        set
        {
            if (value == Color.Empty)
                return;

            deadBrush.Color = D2DColor.FromGDIColor(value);
            Render(DrawBoard);
        }
    }

    public Color GridColor
    {
        get => D2DColor.ToGDIColor(gridPen.Color);
        set
        {
            if (value == Color.Empty)
                return;

            //gridPen.Color = D2DColor.FromGDIColor(value);

            if (ShowGrid)
                Render(DrawGrid);
        }
    }
    #endregion

    public BoardPanel()
    {
        InitializeComponent();

        aliveBrush = Device.CreateSolidColorBrush(D2DColor.FromGDIColor(DefaultAliveColor))!;
        deadBrush = Device.CreateSolidColorBrush(D2DColor.FromGDIColor(DefaultAliveColor))!;
        gridPen = Device.CreatePen(D2DColor.FromGDIColor(DefaultGridColor))!;
        oldSize = Size;
        _board = new(defaultBoardSize, defaultBoardSize);
    }

    public void SetAllColors(Color alive, Color dead, Color grid, bool redraw = true)
    {
        aliveBrush.Color = D2DColor.FromGDIColor(alive);
        deadBrush.Color = D2DColor.FromGDIColor(dead);
        //gridPen.Color = D2DColor.FromGDIColor(grid);

        if (redraw)
            Render(DrawBoard);
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
    public Size AdjustSize()
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

        Render(() =>
        {
            if (ShowGrid)
                DrawGrid();

            DrawBoard();
        });

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
        void HandleChange(object? _, Point p) => UpdateCell(p.X, p.Y);

        Board.StatusChanged += HandleChange;
        Board.Cycle();
        Render(DrawBoard);
    }

    #region Drawing
    private void Render(Action renderAction)
    {
        graphics?.BeginRender();
        renderAction();
        graphics?.EndRender();
    }

    /// <summary>
    /// Erases and fully draws the current board state.
    /// </summary>
    private void DrawBoard()
    {
        DrawBoardRegion(new Rectangle(Point.Empty, Size));
    }

    /// <summary>
    /// Draws a region of the board state using an existing <see cref="Graphics"/> instance.
    /// </summary>
    /// <param name="pixelRegion">Region to draw in pixels</param>
    private void DrawBoardRegion(Rectangle pixelRegion)
    {
        graphics?.FillRectangle(pixelRegion, deadBrush);

        var cellRegion = MapPixelRegionToCellRegion(pixelRegion);

        if (ShowGrid)
            DrawGridRegion(cellRegion);

        for (int x = cellRegion.Left; x < cellRegion.Right && x < Board.Width; x++)
            for (int y = cellRegion.Top; y < cellRegion.Bottom && y < Board.Height; y++)
                if (Board[x, y])
                    DrawAliveCell(x, y);
    }

    private void DrawAliveCell(int x, int y) => DrawCellBase(x, y, aliveBrush);
    private void DrawDeadCell(int x, int y) => DrawCellBase(x, y, deadBrush);
    private void DrawCellBase(int x, int y, D2DBrush brush)
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

        graphics?.FillRectangle(new D2DRect(x, y, width, height), brush);
    }

    private void InvertCell(int x, int y)
    {
        Board.InvertCell(x, y);
        Render(() => UpdateCell(Math.Clamp(x, 0, Board.Width - 1), Math.Clamp(y, 0, Board.Height - 1)));
    }

    private void UpdateCell(int x, int y)
    {
        if (Board[x, y])
            DrawAliveCell(x, y);
        else
            DrawDeadCell(x, y);
    }

    /// <summary>
    /// Draws the full grid.
    /// </summary>
    private void DrawGrid()
    {
        DrawGridRegion(new(Point.Empty, Board.Size));
    }

    /// <summary>
    /// Draws the grid in a region of the panel.
    /// </summary>
    /// <param name="cellRegion">Region to draw in cells</param>
    private void DrawGridRegion(Rectangle cellRegion)
    {
        for (int x = cellRegion.Left; x <= cellRegion.Right; x += 2)
            graphics?.DrawRectangle(new D2DRect(x * CellSize, 0, CellSize, cellRegion.Bottom * CellSize), gridPen);
        for (int y = cellRegion.Top; y <= cellRegion.Bottom; y += 2)
            graphics?.DrawRectangle(new D2DRect(0, y * CellSize, cellRegion.Right * CellSize, CellSize), gridPen);
    }
    #endregion

    #region EventHandlers
    protected override void OnRender(D2DGraphics g)
    {
        graphics ??= g;

        DrawGrid();
        DrawBoard();
    }

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

        Board.InvertCell(cell.X, cell.Y);

        Render(() => DrawCellBase(cell.X, cell.Y, Board[cell.X, cell.Y] ? aliveBrush : deadBrush));
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
