using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeGenerator
{
    internal class DisjointSet
    {
        // Count of elements.
        public int Count { get; }

        // Collection with the parent of each element.
        private int[] Parent;
        private int[] Rank;

        // Create a new disjoint set data structure.
        public DisjointSet(int count)
        {
            Count = count;
            Parent = new int[Count];
            Rank = new int[Count];

            for (var i = 0; i < Count; i++)
            {
                Parent[i] = i;
                Rank[i] = i;
            }
        }

        // Find an element.
        public int Find(int i)
        {
            // If we are the parent itself, return i.
            if (Parent[i] == i)
            {
                return i;
            }

            // Find the root node recursively.
            var result = Find(Parent[i]);

            // Add i, which we were looking for initially, to this root node.
            Parent[i] = result;

            return result;
        }

        // Return the parent.
        public int GetParent(int i) => Parent[i];

        // Union two elements.
        public void Union(int i, int j)
        {
            // Find the root, and ranks.
            var iRoot = Find(i);
            var jRoot = Find(j);
            var iRank = Rank[iRoot];
            var jRank = Rank[jRoot];

            // If they are already in the same set, we don't do a thing.
            if (iRoot == jRoot) return;

            // Union to get the least depth.
            if (iRank < jRank)
            {
                Parent[iRoot] = jRoot;
            }
            else if (iRank > jRank)
            {
                Parent[jRoot] = iRoot;
            }
            else
            {
                // They are equal, so union, and increase the rank.
                Parent[iRoot] = jRoot;
                Rank[jRoot]++;
            }
        }
    }
}
