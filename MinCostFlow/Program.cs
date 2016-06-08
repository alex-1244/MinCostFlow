using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinCostFlow
{
    class Program
    {
        static void Main(string[] args)
        {
            int neededFlow = 15;
            int[,] flows = {
                {0,5,6,0,0,7,0 },
                {0,0,5,4,7,8,6 },
                {0,0,0,5,5,8,5 },
                {0,0,0,0,5,5,5 },
                {0,0,0,0,0,6,5 },
                {0,0,0,0,0,0,9 },
                {0,0,0,0,0,0,0 }
            };
            int[,] costs = {
                {0,1,2,0,0,3,0 },
                {0,0,5,4,7,8,6 },
                {0,0,0,5,4,2,5 },
                {0,0,0,0,5,1,5 },
                {0,0,0,0,0,1,4 },
                {0,0,0,0,0,0,3 },
                {0,0,0,0,0,0,0 }
            };

            Milestone[,] costsMatrix = ConvertToMilestones(costs);
            CalculateFlow(neededFlow, flows, costs);
        }

        private static flowCostPath CalculateFlow(int neededFlow, int[,] flows, int[,] costs)
        {


            return new flowCostPath();
        }

        public static bool bfs(int[,] rGraph, int s, int t, int[] parent)
        {
            // Create a visited array and mark all vertices as not visited
            int V = rGraph.GetLength(0) - 1;
            bool[] visited = new bool[V];

            // Create a queue, enqueue source vertex and mark source vertex
            // as visited
            Queue q = new Queue();
            q.Enqueue(s);
            visited[s] = true;
            parent[s] = -1;

            // Standard BFS Loop
            while (q.Count != 0)
            {
                int u = (int)q.Dequeue();

                for (int v = 0; v < V; v++)
                {
                    if (visited[v] == false && rGraph[u, v] > 0)
                    {
                        q.Enqueue(v);
                        parent[v] = u;
                        visited[v] = true;
                    }
                }
            }

            // If we reached sink in BFS starting from source, then return
            // true, else false
            return (visited[t] == true);
        }

        public static Milestone[,] ConvertToMilestones(int[,] costs)
        {
            int size = costs.GetLength(0);
            Milestone[,] res = new Milestone[size, size];
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                {
                    res[i, j] = new Milestone { InitialFlow = costs[i, j] };
                }
            return res;
        }
    }
    class flowCostPath
    {
        int flow;
        int cost;
        int[] path;
    }

    class Milestone
    {
        public int InitialFlow { get; set; }
        public int remainingFlow { get; set; }
        public int cost { get; set; }
        public Milestone prevMilestone { get; set; }

        public int getTotalCost()
        {
            var maxFlow = this.InititalFlow;
            var totalCost = 0;
            var prevMilestone = this.prevMilestone;
            while (prevMilestone != null)
            {
                maxFlow = Math.Min(maxFlow, prevMilestone.InititalFlow);
                prevMilestone = prevMilestone.prevMilestone;
            }
            prevMilestone = this.prevMilestone;
            while (prevMilestone != null)
            {
                totalCost += prevMilestone.cost * maxFlow;
                prevMilestone = this.prevMilestone;
            }
            return totalCost;
        }
    }
}
