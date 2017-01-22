using MazeGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeApp
{
    public class Maze
    {
        // Width of the maze.
        public int Width { get; }

        // Height of the maze.
        public int Height { get; }

        // Cells of the maze.
        private DisjointSet _vertices { get; }

        // Walls of the maze.
        private List<Edge> _edges { get; }

        #region Events
        public delegate void Change();
        public Change OnChange;

        public delegate void Completed();
        public Completed OnCompleted;
        #endregion

        // Lock for multithreaded genering & drawing.
        private object _lock = false;

        // Initialize a maze with all the walls.
        public Maze(int width, int height)
        {
            Width = width;
            Height = height;

            _vertices = new DisjointSet(Width * Height);
            _edges = new List<Edge>();

            for (int i = 0; i < _vertices.Count; i++)
            {
                // Add the east edge.
                if ((i + 1) % width != 0 || i == 0)
                {
                    _edges.Add(new Edge { From = i, To = i + 1 });
                }

                // Add the south edge.
                if (i + width < _vertices.Count)
                {
                    _edges.Add(new Edge { From = i, To = i + width });
                }
            }
        }

        // Return the number for any given cell in the maze in the coordinate plane.
        public int this[int x, int y]
        {
            get
            {
                if (x < 0 || x > Width) throw new IndexOutOfRangeException(nameof(x));
                if (y < 0 || y > Height) throw new IndexOutOfRangeException(nameof(y));

                return y * Width + x;
            }
        }

        // Check if two cells have a wall inbetween.
        public bool IsWallBetween(int from, int to)
        {
            lock (_lock)
                return _edges.Any(e => (e.From == from && e.To == to)
                                    || (e.From == to   && e.To == from));
        }

        // Return the total of cells.
        public int TotalVertices => _vertices.Count;

        // Return the total of edges.
        public int TotalEdges => _edges.Count;

        // Kruskal's algorithm.
        public void ApplyKruskals()
        {
            var rng = new Random();
            var last = _vertices.Count - 1;

            // While E is not empty, and F is not yet spanning.
            while (_edges.Count > 0 && !(_vertices.Find(0) == _vertices.Find(last)))
            {
                // Select a random edge.
                var edge = _edges[rng.Next(0, _edges.Count)];

                // If the removed edge connects two different trees.
                if (_vertices.Find(edge.From) != _vertices.Find(edge.To))
                {
                    // Connect the trees.
                    _vertices.Union(edge.From, edge.To);

                    // Remove the edge.
                    lock(_lock)
                        _edges.Remove(edge);

                    // Notify that the maze has changed.
                    InvokeOnChange();
                }
            }

            // Notify the maze is completed.
            InvokeOnCompleted();
        }

        #region Invoke events
        private void InvokeOnChange() => OnChange?.Invoke();
        private void InvokeOnCompleted() => OnCompleted?.Invoke();
        #endregion
    }
}
