namespace GameOfLife;

public partial class MainForm : Form
{
    const int defaultWidth = 25, defaultHeight = 25;
    private static readonly Color defaultAliveColor = Color.Black, defaultDeadColor = Color.White, defaultGridColor = Color.Gray;

    private bool simulationRunning, showGrid = true;
    private int cellSize = 0;
    private readonly Size panelFormOffset;
    private Size oldPanelSize;
    private Point dragLastPosition;
    private bool dragInitialState;
    private SolidBrush aliveBrush, deadBrush;
    private Pen gridPen;
    private Board board;

    public MainForm()
    {
        InitializeComponent();

        var presetColors = new Color[]
        {
            Color.White,
            Color.Black,
            Color.Red,
            Color.Green,
            Color.Blue,
            Color.Yellow,
            Color.Orange,
            Color.Purple,
            Color.Pink
        };

        AddColorOptions(Menu_Color_Alive, Menu_Color_Alive_Preset_Click);
        AddColorOptions(Menu_Color_Dead, Menu_Color_Dead_Preset_Click);
        AddColorOptions(Menu_Color_Grid, Menu_Color_Grid_Preset_Click);

        void AddColorOptions(ToolStripMenuItem item, EventHandler clickHandler)
        {
            for (int i = 0; i < presetColors.Length; i++)
            {
                var color = presetColors[i];
                var presetItem = new ToolStripMenuItem(color.Name) { Tag = color };

                presetItem.Click += clickHandler;

                item.DropDownItems.Insert(i, presetItem);
            }
        }

        panelFormOffset = new Size(Width - panBoard.Width, Height - panBoard.Height);

        aliveBrush = new(defaultAliveColor);
        deadBrush = new(defaultDeadColor);
        gridPen = new(defaultGridColor);

        SetBoardSize(defaultWidth, defaultHeight);
        SetCellSizeFormSize();
        SizeFormToBoard();
        DrawGrid();
    }

    #region Coordinate mapping
    /// <summary>
    /// Maps a board pixel to a cell position.
    /// </summary>
    private Point MapPixelToCell(int x, int y) => new(x / cellSize, y / cellSize);
    /// <inheritdoc cref="MapPixelToCell(int, int)"/>
    private Point MapPixelToCell(Point pixel) => MapPixelToCell(pixel.X, pixel.Y);

    /// <summary>
    /// Maps a pixel region to a cell region.
    /// </summary>
    private Rectangle MapPixelRegionToCellRegion(Rectangle region)
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
    /// <summary>
    /// Sets the size of the board.
    /// </summary>
    private void SetBoardSize(int width, int height)
    {
        board = new(width, height);
        Text = $"Game of Life ({width}x{height})";
    }
    /// <summary>
    /// Adjusts the cell size to closely match the size of the panel.
    /// </summary>
    private void SetCellSizeFormSize()
    {
        var widthBasedCellSize = (int)Math.Round(panBoard.Width / (double)board.Width);
        var heightBasedCellSize = (int)Math.Round(panBoard.Height / (double)board.Height);

        var widthBasedDelta = GetDelta(widthBasedCellSize);
        var heightBasedDelta = GetDelta(heightBasedCellSize);

        cellSize = widthBasedDelta < heightBasedDelta ? widthBasedCellSize : heightBasedCellSize;

        int GetDelta(int cellSize) => Math.Abs(panBoard.Width - board.Width * cellSize) + Math.Abs(panBoard.Height - board.Height * cellSize);
    }
    /// <summary>
    /// Adjusts the cell size to closely match the size of the panel based on a resize.
    /// </summary>
    private void SetCellSizeFormSize(Size oldBoardSize)
    {
        // If only one dimension changed, base the cell size on that dimension
        if (oldBoardSize.Width != panBoard.Width)
        {
            if (oldBoardSize.Height == panBoard.Height)
                cellSize = panBoard.Width / board.Width;
            else
                SetCellSizeFormSize();
        }
        else if (oldBoardSize.Height != panBoard.Height)
            cellSize = panBoard.Height / board.Height;
        else
            SetCellSizeFormSize();
    }

    /// <summary>
    /// Resizes the form to the panel matches the board size.
    /// </summary>
    private void SizeFormToBoard()
    {
        oldPanelSize = new(board.Width * cellSize, board.Height * cellSize);
        Size = oldPanelSize + panelFormOffset;
    }
    #endregion

    #region Drawing
    /// <summary>
    /// Erases the board.
    /// </summary>
    private void Erase()
    {
        using var g = panBoard.CreateGraphics();
        Erase(g);

        if (showGrid)
            DrawGrid();
    }
    /// <summary>
    /// Erases the board using an existing <see cref="Graphics"/> instance.
    /// </summary>
    private void Erase(Graphics g) => g.FillRectangle(deadBrush, new(Point.Empty, panBoard.Size));

    /// <summary>
    /// Erases and fully draws the current board state.
    /// </summary>
    private void DrawBoard()
    {
        using var g = panBoard.CreateGraphics();
        DrawBoard(g);
    }
    /// <summary>
    /// Fully draws the current board state using an existing <see cref="Graphics"/> instance.
    /// </summary>
    private void DrawBoard(Graphics g) => DrawBoardRegion(new(Point.Empty, panBoard.Size), g);
    /// <summary>
    /// Draws a region of the board state using an existing <see cref="Graphics"/> instance.
    /// </summary>
    /// <param name="pixelRegion">Region to draw in pixels</param>
    private void DrawBoardRegion(Rectangle pixelRegion, Graphics g)
    {
        g.FillRectangle(deadBrush, pixelRegion);

        var cellRegion = MapPixelRegionToCellRegion(pixelRegion);

        for (int x = cellRegion.Left; x < cellRegion.Right && x < board.Width; x++)
            for (int y = cellRegion.Top; y < cellRegion.Bottom && y < board.Height; y++)
                if (board.Cells[x, y])
                    DrawAliveCell(x, y, g, noGrid: true);

        if (showGrid)
            DrawGridRegion(cellRegion, g);
    }

    private void UpdateBoard()
    {
        using var g = panBoard.CreateGraphics();

        for (int x = 0; x < board.Width; x++)
            for (int y = 0; y < board.Height; y++)
                if (board.OldCells[x, y] != board.Cells[x, y])
                    UpdateCell(x, y, g);
    }

    private void DrawAliveCell(int x, int y, Graphics g, bool noGrid = false) => DrawCellBase(x, y, g, aliveBrush, noGrid);
    private void DrawDeadCell(int x, int y, Graphics g, bool noGrid = false) => DrawCellBase(x, y, g, deadBrush, noGrid);
    private void DrawCellBase(int x, int y, Graphics g, Brush brush, bool noGrid)
    {
        g.FillRectangle(brush, x * cellSize, y * cellSize, cellSize, cellSize);

        if (showGrid && !noGrid)
            DrawGridSpace(x, y, g);
    }

    private void InvertCell(int x, int y)
    {
        using var g = panBoard.CreateGraphics();

        board.InvertCell(x, y);
        UpdateCell(Math.Clamp(x, 0, board.Width - 1), Math.Clamp(y, 0, board.Height - 1), g);
        UpdateStatus();
    }
    private void UpdateCell(int x, int y, Graphics g, bool noGrid = false)
    {
        if (board.Cells[x, y])
            DrawAliveCell(x, y, g, noGrid);
        else
            DrawDeadCell(x, y, g, noGrid);
    }

    /// <summary>
    /// Draws the full grid.
    /// </summary>
    private void DrawGrid()
    {
        using var g = panBoard.CreateGraphics();
        DrawGridRegion(new(Point.Empty, board.Size), g);
    }
    /// <summary>
    /// Draws the sides of a grid space.
    /// </summary>
    private void DrawGridSpace(int x, int y, Graphics g) => g.DrawRectangle(gridPen, x * cellSize, y * cellSize, cellSize, cellSize);
    /// <summary>
    /// Draws the grid in a region of the panel.
    /// </summary>
    /// <param name="cellRegion">Region to draw in cells</param>
    private void DrawGridRegion(Rectangle cellRegion, Graphics g)
    {
        for (int x = cellRegion.Left; x <= cellRegion.Right; x++)
            g.DrawRectangle(gridPen, x * cellSize, 0, cellSize, cellRegion.Bottom * cellSize);
        for (int y = cellRegion.Top; y <= cellRegion.Bottom; y++)
            g.DrawRectangle(gridPen, 0, y * cellSize, cellRegion.Right * cellSize, cellSize);
    }
    #endregion

    #region Colors
    /// <summary>
    /// Prompts the user to pick a color.
    /// </summary>
    /// <param name="oldColor">Starting color selection</param>
    /// <param name="newColor">Color selected by the user or oldColor if the user cancels the selection</param>
    /// <returns><see langword="true"/> if the user confirmed the color selection</returns>
    private static bool SelectColor(Color oldColor, out Color newColor)
    {
        using ColorDialog diag = new() { Color = oldColor };
        var result = diag.ShowDialog();

        newColor = diag.Color;
        return result == DialogResult.OK;
    }
    #endregion

    #region Simulation control
    /// <summary>
    /// Starts the simulation.
    /// </summary>
    private void StartSimulation()
    {
        simulationRunning = true;
        Menu_StartStop.Text = "Stop";
        Timer.Start();
    }
    /// <summary>
    /// Stops the simulation.
    /// </summary>
    private void StopSimulation()
    {
        simulationRunning = false;
        Menu_StartStop.Text = "Start";
        Timer.Stop();
    }
    #endregion

    private void UpdateStatus()
    {
        Status_State.Text = Status_State.Text = $"Gen: {board.Generation} Pop: {board.Population}";
    }

    #region Event handlers
    private void panBoard_MouseDown(object sender, MouseEventArgs e)
    {
        var cell = MapPixelToCell(e.Location);
        using var g = panBoard.CreateGraphics();

        dragInitialState = board.Cells[cell.X, cell.Y];
        InvertCell(cell.X, cell.Y);

        panBoard.MouseMove += panBoard_MouseMove;
    }
    private void panBoard_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.Location.X < 0 || e.Location.X >= panBoard.Width || e.Location.Y < 0 || e.Location.Y >= panBoard.Height)
            return;

        var cell = MapPixelToCell(e.Location);

        if (cell == dragLastPosition || board.Cells[cell.X, cell.Y] != dragInitialState)
            return;

        InvertCell(cell.X, cell.Y);
    }
    private void panBoard_MouseUp(object sender, MouseEventArgs e) => panBoard.MouseMove -= panBoard_MouseMove;

    private void panBoard_Paint(object sender, PaintEventArgs e) => DrawBoardRegion(e.ClipRectangle, e.Graphics);

    private void MainForm_ResizeEnd(object sender, EventArgs e)
    {
        SetCellSizeFormSize(oldPanelSize);
        SizeFormToBoard();
        DrawBoard();
    }

    private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
    {
        aliveBrush.Dispose();
        deadBrush.Dispose();
        gridPen.Dispose();
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        board.Cycle();
        UpdateBoard();
        UpdateStatus();
    }

    #region Menu bar
    private void Menu_StartStop_Click(object sender, EventArgs e)
    {
        if (simulationRunning)
            StopSimulation();
        else
            StartSimulation();
    }

    private void Menu_Reset_Click(object sender, EventArgs e)
    {
        StopSimulation();

        board = new(board.Size);
        Erase();
    }

    private void Menu_BoardSize_Click(object sender, EventArgs e)
    {
        using var form = new ResizeForm(board.Width, board.Height);

        if (form.ShowDialog() == DialogResult.OK)
        {
            SetBoardSize(form.BoardWidth, form.BoardHeight);
            SetCellSizeFormSize();
            SizeFormToBoard();
            Erase();
        }
    }

    private void Menu_ShowGrid_Click(object sender, EventArgs e)
    {
        Menu_ShowGrid.Checked = showGrid = !showGrid;
        Menu_ShowGrid.Text = showGrid ? "Show Grid ✓" : "Show Grid";

        DrawBoard();
    }

    private void Menu_Color_Alive_Preset_Click(object sender, EventArgs e)
    {
        aliveBrush.Color = (Color)(sender as ToolStripMenuItem)!.Tag;
        DrawBoard();
    }
    private void Menu_Color_Dead_Preset_Click(object sender, EventArgs e)
    {
        deadBrush.Color = (Color)(sender as ToolStripMenuItem)!.Tag;
        DrawBoard();
    }

    private void Menu_Color_Grid_Preset_Click(object sender, EventArgs e)
    {
        gridPen.Color = (Color)(sender as ToolStripMenuItem)!.Tag;
        DrawBoard();
    }

    private void Menu_Color_Alive_Custom_Click(object sender, EventArgs e)
    {
        if (SelectColor(aliveBrush.Color, out var newColor))
        {
            aliveBrush.Color = newColor;
            DrawBoard();
        }
    }
    private void Menu_Color_Dead_Custom_Click(object sender, EventArgs e)
    {
        if (SelectColor(deadBrush.Color, out var newColor))
        {
            deadBrush.Color = newColor;
            DrawBoard();
        }
    }
    private void Menu_Color_Grid_Custom_Click(object sender, EventArgs e)
    {
        if (SelectColor(deadBrush.Color, out var newColor))
        {
            gridPen.Color = newColor;
            DrawGrid();
        }
    }

    private void Menu_Colors_Reset_Click(object sender, EventArgs e)
    {
        aliveBrush.Color = defaultAliveColor;
        deadBrush.Color = defaultDeadColor;
        gridPen.Color = defaultGridColor;

        DrawBoard();
    }
    #endregion
    #endregion
}
