namespace GameOfLife;

public partial class SpeedForm : Form
{
    public int TickRate { get; private set; }

    public SpeedForm(int tickRate)
    {
        InitializeComponent();

        numTickRate.Value = tickRate;
    }

    private void Button_Click(object sender, EventArgs e)
    {
        TickRate = (int)numTickRate.Value;

        var button = sender as Button;
        DialogResult = button!.DialogResult;
    }
}
