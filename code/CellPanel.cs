using System.Drawing;
using System.Windows.Forms;

namespace GameOfLife
{
    public partial class CellPanel : Panel
    {
        public static Color AliveColor { get; set; }
        public static Color DeadColor { get; set; }

        public Point CellLocation { get; init; }

        private bool _alive;
        public bool Alive
        {
            get => _alive;
            set
            {
                _alive = value;
                BackColor = _alive ? AliveColor : DeadColor;
            }
        }

        public CellPanel() => InitializeComponent();
    }
}
