using System.Windows.Forms;

namespace GameOfLife
{
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

        private void ResizeForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            BoardWidth = (int)WidthInput.Value;
            BoardHeight = (int)HeightInput.Value;
        }
    }
}
