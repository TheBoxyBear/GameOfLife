namespace GameOfLife;

public partial class ResizeForm : Form
{
    public int BoardWidth { get; private set; }
    public int BoardHeight { get; private set; }

    public ResizeForm(int boardWidth, int boardHeight)
    {
        InitializeComponent();

        WidthInput.Value = BoardWidth = boardWidth;
        HeightInput.Value = BoardHeight = boardHeight;
    }

    private void Btn_Click(object sender, EventArgs e)
    {
        BoardWidth = (int)WidthInput.Value;
        BoardHeight = (int)HeightInput.Value;

        var button = sender as Button;
        DialogResult = button!.DialogResult;
    }
}
