namespace GameOfLife;

public partial class MainForm : Form
{
    private bool simulationRunning;
    private Size previousSize;
    private readonly Size panelFormOffset;

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
        panBoard.ResetColors(false);
        AdjustSize();
        UpdateSizeText();
    }

    #region Sizing
    private void AdjustSize()
    {
        panBoard.AdjustSize();
        SizeToPanel();
    }
    private void SizeToPanel() => Size = panBoard.Size + panelFormOffset;
    /// <summary>
    /// Sets the size of the board.
    /// </summary>
    private void SetBoardSize(int width, int height)
    {
        panBoard.Board = new(width, height);
        SizeToPanel();
        UpdateSizeText();
    }
    private void UpdateSizeText() => Text = $"Game of Life ({panBoard.Board.Width}x{panBoard.Board.Height})";
    private void UpdateStatusText() => Status_State.Text = $"Gen: {panBoard.Board.Generation} Pop: {panBoard.Board.Population}";
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

    public void Cycle()
    {
        panBoard.Cycle();
        UpdateStatusText();
    }

    #region Event handlers
    private void MainForm_Resize(object sender, EventArgs e)
    {
        if (Size == previousSize)
            return;

        previousSize = Size;
        panBoard.Size = Size - panelFormOffset;
    }

    private void MainForm_ResizeEnd(object sender, EventArgs e) => AdjustSize();
    private void Timer_Tick(object sender, EventArgs e) => Cycle();

    #region Menu bar
    private void Menu_StartStop_Click(object sender, EventArgs e)
    {
        if (simulationRunning)
            StopSimulation();
        else
            StartSimulation();
    }

    private void Menu_Speed_Click(object sender, EventArgs e)
    {
        using var frm = new SpeedForm(Timer.Interval);

        if (frm.ShowDialog() == DialogResult.OK)
            Timer.Interval = frm.TickRate;
    }

    private void Menu_Reset_Click(object sender, EventArgs e)
    {
        StopSimulation();
        panBoard.Board = new(panBoard.Board.Size);
        UpdateStatusText();
    }

    private void Menu_BoardSize_Click(object sender, EventArgs e)
    {
        using var form = new ResizeForm(panBoard.Board.Width, panBoard.Board.Height);

        if (form.ShowDialog() == DialogResult.OK)
            SetBoardSize(form.BoardWidth, form.BoardHeight);
    }

    private void Menu_ShowGrid_Click(object sender, EventArgs e)
    {
        Menu_ShowGrid.Checked = panBoard.ShowGrid = !panBoard.ShowGrid;
        Menu_ShowGrid.Text = panBoard.ShowGrid ? "Show Grid ✓" : "Show Grid";
    }

    private void Menu_Color_Alive_Preset_Click(object? sender, EventArgs e) => panBoard.AliveColor = (Color)(sender as ToolStripMenuItem)!.Tag;
    private void Menu_Color_Dead_Preset_Click(object? sender, EventArgs e) => panBoard.DeadColor = (Color)(sender as ToolStripMenuItem)!.Tag;
    private void Menu_Color_Grid_Preset_Click(object? sender, EventArgs e) => panBoard.GridColor = (Color) (sender as ToolStripMenuItem)!.Tag;

    private void Menu_Color_Alive_Custom_Click(object sender, EventArgs e)
    {
        if (SelectColor(panBoard.AliveColor, out var newColor))
            panBoard.AliveColor = newColor;
    }

    private void Menu_Color_Dead_Custom_Click(object sender, EventArgs e)
    {
        if (SelectColor(panBoard.DeadColor, out var newColor))
            panBoard.DeadColor = newColor;
    }

    private void Menu_Color_Grid_Custom_Click(object sender, EventArgs e)
    {
        if (SelectColor(panBoard.GridColor, out var newColor))
            panBoard.GridColor = newColor;
    }

    private void Menu_Colors_Reset_Click(object sender, EventArgs e) => panBoard.ResetColors(true);
    #endregion
    #endregion
}
