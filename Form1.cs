using MazeGenerator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MazeApp
{
    public partial class Form1 : Form
    {
        // The maze object.
        private Maze _maze { get; set; }

        // Thickness of the walls.
        private const int _wallSize = 1;

        // Size of the cells (padding).
        private int _cellSize { get; set; } = 25;

        // Debug grid, used during development.
        private bool _showGrid { get; set; } = false;

        // Maze as bitmap.
        private Bitmap _mazeImage { get; set; }

        // Speed of redrawing.
        private RedrawSpeed _speed { get; set; } = RedrawSpeed.Slow;

        public Form1()
        {
            InitializeComponent();
            toolStripStatusLabel1.Text = "...";
            toolStripStatusLabel2.Text = "Initializing...";
            toolStripStatusLabel3.Text = "...";
            Generate();
        }

        // Generate a new maze.
        private void Generate()
        {
            // Create the maze.
            _maze = new Maze(10, 10);
            _maze.OnChange += Maze_OnChange;
            _maze.OnCompleted += Maze_OnCompleted;
            Task.Run(() => _maze.ApplyKruskals());
        }

        private void UpdateStatusBar(string status)
        {
            // Debug information
            toolStripStatusLabel1.Text = $"Nodes: {_maze.TotalVertices}  Edges: {_maze.TotalEdges}";
            toolStripStatusLabel2.Text = status;
            toolStripStatusLabel3.Text = $"Size: {_maze.Width}x{_maze.Height}  Speed: {_speed}";
        }

        // Redraw the maze when it changes.
        private void Maze_OnChange()
        {
            if (_speed == RedrawSpeed.Off) return;
            if (_speed == RedrawSpeed.Fast && _maze.TotalEdges % 25 != 0) return;

            _mazeImage = Draw(_maze, _wallSize, _cellSize, showGrid: _showGrid);
            pictureBox1.Image = _mazeImage;
            UpdateStatusBar("Generating...");
        }

        private void Maze_OnCompleted()
        {
            _mazeImage = Draw(_maze, _wallSize, _cellSize, showGrid: _showGrid);
            pictureBox1.Image = _mazeImage;
            UpdateStatusBar("Completed!");
        }

        // Draw the maze as a bitmap. This method is really slow so please don't make the 
        // maze too big, or you will have to wait forever for a redraw.
        private Bitmap Draw(Maze maze, int wallSize, int cellSize, bool showGrid = false)
        {
            // Dimensions
            int bitmapWidth = wallSize + (wallSize + cellSize) * maze.Width;
            int bitmapHeight = wallSize + (wallSize + cellSize) * maze.Height;

            var bitmap = new Bitmap(bitmapWidth, bitmapHeight);

            // Make the entire bitmap white, and draw a border.
            for (int i = 0; i < bitmapWidth; i++)
                for (int j = 0; j < bitmapHeight; j++)
                {
                    bitmap.SetPixel(i, j, Color.White);

                    if (i == 0 || j == 0 || i == bitmapWidth - 1 || j == bitmapWidth - 1)
                        bitmap.SetPixel(i, j, Color.Black);
                }


            // Debug grid.
            if (showGrid)
                bitmap.DrawRaster(wallSize, cellSize);

            // Iterate over every cell in the maze.
            for (int i = 0; i < maze.Width; i++)
                for (int j = 0; j < maze.Height; j++)
                {
                    // Get the position of the vertex on the coordinate plane.
                    var point = GetPointForCoordinates(i, j, wallSize, cellSize);

                    for (int k = 0; k < cellSize + wallSize; k++)
                    {
                        // East wall
                        if (maze.IsWallBetween(maze[i, j], maze[i + 1, j]))
                            bitmap.SetPixel(point.X + cellSize, point.Y + k, Color.Black);

                        // South wall
                        if (maze.IsWallBetween(maze[i, j], maze[i, j + 1]))
                            bitmap.SetPixel(point.X + k, point.Y + cellSize, Color.Black);
                    }
                }

            return bitmap;
        }

        // Calculate point in the image for coordinate.
        private Point GetPointForCoordinates(int x, int y, int wallSize, int cellSize)
            => new Point
            {
                X = wallSize + (wallSize + cellSize) * x,
                Y = wallSize + (wallSize + cellSize) * y
            };

        // Holds the X, Y values.
        private struct Point { public int X, Y; }

        // Generate a new maze.
        private void generateToolStripMenuItem_Click(object sender, EventArgs e)
            => Generate();

        // Save the maze to a bitmap.
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dialog = new SaveFileDialog();
            dialog.Filter = "BMP|*.bmp";
            dialog.Title = "Save maze";
            dialog.ShowDialog();

            if (dialog.FileName != "")
            {
                _mazeImage.Save(dialog.FileName);
            }
        }

        // Detect the edges. This will be used in another blog post to solve
        // the maze.
        private void detectEdgesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_mazeImage == null)
            {
                MessageBox.Show("There is not maze.", "Edge detection", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            _mazeImage = new EdgeDetection().Detect(_mazeImage);
            pictureBox1.Image = _mazeImage;
        }

        #region Redrawing speed
        private void offToolStripMenuItem_Click(object sender, EventArgs e)
            => ChangeRedrawingSpeed(sender, RedrawSpeed.Off);

        private void slowToolStripMenuItem_Click(object sender, EventArgs e)
            => ChangeRedrawingSpeed(sender, RedrawSpeed.Slow);

        private void fastToolStripMenuItem_Click(object sender, EventArgs e)
            => ChangeRedrawingSpeed(sender, RedrawSpeed.Fast);

        private enum RedrawSpeed
        {
            Off,
            Slow,
            Fast
        }

        private void ChangeRedrawingSpeed(object sender, RedrawSpeed speed)
        {
            offToolStripMenuItem.Checked = false;
            slowToolStripMenuItem.Checked = false;
            fastToolStripMenuItem.Checked = false;
            ((ToolStripMenuItem)sender).Checked = true;
            _speed = speed;
        }
        #endregion
    }
}
