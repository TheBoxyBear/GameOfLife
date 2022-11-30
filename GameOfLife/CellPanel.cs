namespace GameOfLife;

public partial class CellPanel : Panel
{
    public static readonly Color DefaultAliveColor = Color.Black;
    public static readonly Color DefaultDeadColor = Color.White;

    public static Color AliveColor { get; set; } = DefaultAliveColor;
    public static Color DeadColor { get; set; } = DefaultDeadColor;

    public Point CellLocation { get; init; }

    private bool _isAlive;
    public bool IsAlive
    {
        get => _isAlive;
        set
        {
            _isAlive = value;
            UpdateColor();
        }
    }

    public CellPanel()
    {
        InitializeComponent();
        IsAlive = false;
    }

    public void UpdateColor() => BackColor = _isAlive ? AliveColor : DeadColor;
}
