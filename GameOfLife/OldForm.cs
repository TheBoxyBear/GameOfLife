namespace GameOfLife;

public partial class OldForm : Form
{
    private bool simulationRunning = false;
    private Board board = new(10, 10);
    private CellPanel[][] cells;

    public OldForm()
    {
        InitializeComponent();
        InitBoardGUI();
    }

    #region Simulation control
    private void StartSimulation()
    {
        Timer.Start();

        simulationRunning = true;
        Menu_StartStop.Text = "Stop";
    }
    private void StopSimulation()
    {
        Timer.Stop();

        simulationRunning = false;
        Menu_StartStop.Text = "Start";
    }
    #endregion

    #region Events
    private void Menu_StartStop_Click(object sender, EventArgs e)
    {
        if (simulationRunning)
            StopSimulation();
        else
            StartSimulation();
    }
    private void Cell_Click(object sender, EventArgs e)
    {
        if (!simulationRunning)
        {
            var cell = sender as CellPanel;
            board.Cells[cell!.CellLocation.X, cell.CellLocation.Y] = cell.IsAlive = !cell.IsAlive;
        }
    }
    private void Menu_Reset_Click(object sender, EventArgs e)
    {
        StopSimulation();

        foreach (CellPanel cell in cells.SelectMany(c => c))
            board.Cells[cell.CellLocation.X, cell.CellLocation.Y] = cell.IsAlive = false;
    }
    private void Timer_Tick(object sender, EventArgs e)
    {
        board.Cycle();

        // Update the panels
        foreach (CellPanel cell in cells.SelectMany(c => c))
            cell.IsAlive = board.Cells[cell.CellLocation.X, cell.CellLocation.Y];
    }


    private void Menu_BoardSize_Click(object sender, EventArgs e)
    {
        StopSimulation();
        Hide();

        using ResizeForm frm = new(board.Size.Width, board.Size.Height);
        frm.ShowDialog();

        board = new(frm.BoardWidth, frm.BoardHeight);
        InitBoardGUI();

        Show();
    }

    private void Menu_Color_Alive_Click(object sender, EventArgs e)
    {
        if (ShowColorDiag(CellPanel.AliveColor, out Color newColor))
        {
            CellPanel.AliveColor = newColor;

            foreach (CellPanel cell in cells.SelectMany(c => c).Where(c => c.IsAlive))
                cell.UpdateColor();
        }
    }
    private void Menu_Color_Dead_Click(object sender, EventArgs e)
    {
        if (ShowColorDiag(CellPanel.DeadColor, out Color newColor))
        {
            CellPanel.DeadColor = newColor;

            foreach (CellPanel cell in cells.SelectMany(c => c).Where(c => !c.IsAlive))
                cell.UpdateColor();
        }
    }
    private void Menu_Colors_Reset_Click(object sender, EventArgs e)
    {
        CellPanel.AliveColor = CellPanel.DefaultAliveColor;
        CellPanel.DeadColor = CellPanel.DefaultDeadColor;

        foreach (CellPanel cell in TableBoard.Controls)
            cell.UpdateColor();
    }
    #endregion

    /// <summary>
    /// Prompts the user to pick a color.
    /// </summary>
    /// <param name="oldColor">Starting color selection</param>
    /// <param name="newColor">Color selected by the user or oldColor if the user cancels the selection</param>
    /// <returns><see langword="true"/> if the user confirmed the color selection</returns>
    private static bool ShowColorDiag(Color oldColor, out Color newColor)
    {
        using ColorDialog diag = new() { Color = oldColor };

        if (diag.ShowDialog() == DialogResult.OK)
        {
            newColor = diag.Color;
            return true;
        }
        else
        {
            newColor = oldColor;
            return false;
        }
    }

    /// <summary>
    /// Resets the table to match the new board.
    /// </summary>
    private void InitBoardGUI()
    {
        // Reset the table
        TableBoard.Controls.Clear();
        TableBoard.RowStyles.Clear();
        TableBoard.ColumnStyles.Clear();

        // Resize the table
        TableBoard.RowCount = board.Size.Width;
        TableBoard.ColumnCount = board.Size.Height;

        float colRatio = 100f / TableBoard.ColumnCount, rowRatio = 100f / TableBoard.RowCount;
        Padding cellMargin = new(0);

        // Initialize the CellPannel jagged array
        cells = new CellPanel[TableBoard.ColumnCount][];
        for (int i = 0; i < cells.Length; i++)
            cells[i] = new CellPanel[TableBoard.RowCount];

        // Add the panels and adjust column sizing
        for (int col = 0; col < TableBoard.ColumnCount; col++)
        {
            TableBoard.ColumnStyles.Add(new(SizeType.Percent, colRatio));

            for (int row = 0; row < TableBoard.RowCount; row++)
            {
                CellPanel panel = new()
                {
                    CellLocation = new(col, row),
                    Dock = DockStyle.Fill,
                    Margin = cellMargin
                };
                panel.Click += Cell_Click;

                cells[col][row] = panel;
                TableBoard.Controls.Add(panel, col, row);
            }
        }

        // Adjust row sizing
        for (int row = 0; row < TableBoard.RowCount; row++)
            TableBoard.RowStyles.Add(new(SizeType.Percent, rowRatio));
    }
}
