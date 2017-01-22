using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeGenerator
{
    // This is no longer used, but this code is used in the blog post so I'll leave 
    // it in case I want to adjust it.
    internal class PrintExamples
    {
        static void Example_CreateAndDisplayVerticesAndEdges()
        {
            var width = 5;
            var height = 5;

            var vertices = new DisjointSet(width * height);

            Console.WriteLine("-- vertices --");
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int index = i * width + j;

                    Console.Write($"{index}".PadLeft(4, ' '));
                }

                Console.WriteLine();
            }

            var edges = new List<Edge>();

            for (int i = 0; i < vertices.Count; i++)
            {
                // Add the east edge.
                if ((i + 1) % width != 0 || i == 0)
                {
                    edges.Add(new Edge { From = i, To = i + 1 });
                }

                // Add the south edge.
                if (i + width < vertices.Count)
                {
                    edges.Add(new Edge { From = i, To = i + width});
                }
            }

            Console.WriteLine("-- edges --");
            foreach (var edge in edges)
            {
                Console.WriteLine($"Edge: {edge.From.ToString().PadLeft(4, ' ')}   -> {edge.To.ToString().PadLeft(4, ' ')}");
            }
        }

        private static void Example_DisplayDisjointSet(DisjointSet set, PrintMethod method = PrintMethod.Root)
        {
            var dictionary = new Dictionary<int, int>();

            for (var i = 0; i < set.Count; i++)
            {
                dictionary.Add(i, method == PrintMethod.Parent ? set.GetParent(i) : set.Find(i));
            }

            var sb = new StringBuilder();

            sb.Append("Parent:".PadLeft(10, ' '));
            foreach (var element in dictionary)
            {
                sb.Append($"{element.Value}".PadLeft(3, ' '));
            }

            sb.AppendLine();
            sb.Append("Element:".PadLeft(10, ' '));
            foreach (var element in dictionary)
            {
                sb.Append($"{element.Key}".PadLeft(3, ' '));
            }

            sb.AppendLine();

            Console.Write(sb.ToString());
        }

        private enum PrintMethod { Root, Parent }
    }
}
