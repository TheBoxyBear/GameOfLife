using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameOfLife
{
    public partial class MainForm : Form
    {
        private bool simulationRunning = false;
        private Board board = new();

        public MainForm()
        {
            InitializeComponent();
            InitBoardGUI();
        }

        #region Simulation control
        private void StartSimulation()
        {
            simulationRunning = true;
            Menu_StartStop.Text = "Stop";
        }
        private void StopSimulation()
        {
            simulationRunning = false;
            Menu_StartStop.Text = "Start";
        }
        #endregion

        #region Events
        private void Menu_BoardSize_Click(object sender, EventArgs e)
        {
            StopSimulation();

            using ResizeForm frm = new(board.Size.Width, board.Size.Height);
            frm.ShowDialog();

            board = new(frm.BoardWidth, frm.BoardHeight);
            InitBoardGUI();
        }
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
                CellPanel cell = sender as CellPanel;
                board.Cells[cell.CellLocation.X, cell.CellLocation.Y] = cell.Alive = !cell.Alive;
            }
        }
        #endregion

        private void InitBoardGUI()
        {
            TableBoard.RowCount = board.Size.Width;
            TableBoard.ColumnCount = board.Size.Height;

            for (int col = 0; col < TableBoard.RowCount; col++)
                for (int row = 0; row < TableBoard.ColumnCount; row++)
                {
                    CellPanel panel = new() { CellLocation = new(col, row) };
                    panel.Click += Cell_Click;
                    TableBoard.Controls.Add(new Panel(), col, row);
                }

        }
    }
}
