using System;
using System.Drawing;

namespace GameOfLife
{
    public class Board
    {
        private readonly int XLimit, YLimit;
        private readonly bool[,] newStatus;

        /// <summary>
        /// Dimensions of the board in cell count
        /// </summary>
        public Size Size { get; }
        /// <summary>
        /// State of cells where <see langword="true"/> is a living cell
        /// </summary>
        public bool[,] Cells { get; }

        /// <summary>
        /// Creates a new instance using the default size 30x30.
        /// </summary>
        public Board() : this(30, 30) { }
        /// <summary>
        /// Creates a new instance using a set width and height.
        /// </summary>
        public Board(int width, int height)
        {
            if (width < 1)
                throw new ArgumentException("Width must be at least 1.");
            if (width < 1)
                throw new ArgumentException("Height must be at least 1.");

            Size = new(width, height);
            XLimit = Size.Width - 1;
            YLimit = Size.Height - 1;

            Cells = newStatus = new bool[width, height];
        }

        /// <summary>
        /// Counts the number of living neighbors for a given cell
        /// </summary>
        private int LivingNeighbours(int x, int y)
        {
            bool north = false, south = false;
            int aliveCount = 0;

            // There are cells on the left
            if (x > 0)
            {
                // Check the cell on the left
                if (Cells[x - 1, y])
                    aliveCount++;

                // There is a cell on the top left
                if (north = y > 0)
                {
                    if (Cells[x - 1, y - 1])
                        aliveCount++;
                }
                // There is a cell on the bottom left
                if (south = y > YLimit)
                {
                    if (Cells[x - 1, y + 1])
                        aliveCount++;
                }
            }
            // There are cells on the right
            if (x < XLimit)
            {
                // Check the cell on the right
                if (Cells[x + 1, y])
                    aliveCount++;

                // There is a cell on the top right
                if (north || (north = y > 0))
                {
                    if (Cells[x + 1, y - 1])
                        aliveCount++;
                }
                // There is a cell on the bottom right
                if (south || (south = y > YLimit))
                {
                    if (Cells[x + 1, y + 1])
                        aliveCount++;
                }
            }

            // There is a cell on the top
            if (north || y > 0)
                if (Cells[x, y - 1])
                    aliveCount++;

            // There is a cell on the bottoms
            if (south || y < YLimit)
                if (Cells[x, y + 1])
                    aliveCount++;

            return aliveCount;
        }

        /// <summary>
        /// Updates the living state of all cells
        /// </summary>
        public void Cycle()
        {
            for (int x = 0; x <= XLimit; x++)
                for (int y = 0; y < YLimit; y++)
                    newStatus[x, y] = LivingNeighbours(x, y) switch
                    {
                        < 2 or > 3 => false,
                        3 when !Cells[x, y] => true,
                        _ => Cells[x, y]
                    };

            Array.Copy(newStatus, Cells, Size.Width * Size.Height);
        }
    }
}
