using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PathfindingLib
{
    public class AdjacencyList : IGraphRepresentation
    {
        public readonly SortedSet<int>[] data;
        public int VertexCount { get; private set; }

        public AdjacencyList(int vertexCount)
        {
            VertexCount = vertexCount;
            data = new SortedSet<int>[vertexCount];

            for (int i = 0; i < vertexCount; ++i)
                data[i] = new SortedSet<int>();
        }

        public void AddEdge(int from, int to)
        {
            data[from].Add(to);
        }

        public void AddEdgeBidirectional(int nodeA, int nodeB)
        {
            AddEdge(nodeA, nodeB);
            AddEdge(nodeB, nodeA);
        }

        public void AddEdges(int from, int[] to)
        {
            for (int i = 0; i < to.Length; ++i)
                AddEdge(from, to[i]);
        }

        public void RemoveEdge(int from, int to)
        {
            data[from].Remove(to);
        }

        public int CountNeighbours(int node) => data[node].Count;
        public bool HasNeighbour(int node, int neighbour) => data[node].Contains(neighbour);
        public IReadOnlyCollection<int> GetNeighbours(int node) => data[node];
    }

    public interface IGraphRepresentation
    {
        public int VertexCount { get; }
        public void AddEdge(int from, int to);
        public void AddEdges(int from, int[] to);
        public void RemoveEdge(int from, int to);
        public bool HasNeighbour(int nodeA, int nodeB);
        public int CountNeighbours(int node);
        public IReadOnlyCollection<int> GetNeighbours(int node);
    }

    public static class Algorithms
    {
        public static List<int> BFS(IGraphRepresentation graph, int startNode, int endNode)
        {
            if (startNode == endNode)
                return new List<int>() { startNode };

            var frontier = new Queue<int>();
            var cameFrom = new int[graph.VertexCount];
            Array.Fill(cameFrom, -1);

            frontier.Enqueue(startNode);

            while (frontier.Count > 0)
            {
                int current = frontier.Dequeue();
                IEnumerable<int> currentNeighbours = graph.GetNeighbours(current);

                for (int i = 0; i < currentNeighbours.Count(); ++i)
                {
                    int next = currentNeighbours.ElementAt(i);

                    if (next == endNode)
                    {
                        cameFrom[next] = current;
                        return BuildShortestPath(startNode, endNode, cameFrom);
                    }

                    if (cameFrom[next] == -1)
                    {
                        frontier.Enqueue(next);
                        cameFrom[next] = current;
                    }
                }
            }

            return new List<int>();
        }

        private static List<int> BuildShortestPath(int startNode, int endNode, int[] cameFrom)
        {
            if (cameFrom[endNode] == -1)
                return new List<int>();
            int current = endNode;
            var shortestPath = new List<int>();

            while (current != startNode)
            {
                shortestPath.Add(current);
                current = cameFrom[current];
            }
            shortestPath.Add(startNode);
            shortestPath.Reverse();
            return shortestPath;
        }
    }
}

