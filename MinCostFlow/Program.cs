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
            CalculateFlow(neededFlow, flows, costsMatrix);
        }

        private static flowCostPath CalculateFlow(int neededFlow, int[,] flows, Milestone[,] costs)
        {


            return new flowCostPath();
        }

        public static MilestoneHist[] bfs(Milestone[,] rGraph, int s, int t)
        {
            int Size = rGraph.GetLength(0) - 1;
            MilestoneHist[] parent = new MilestoneHist[Size];
            bool[] visited = new bool[Size];

            Queue q = new Queue();
            q.Enqueue(s);
            //visited[s] = true;
            parent[s] = null;

            while (q.Count != 0)
            {
                int u = (int)q.Dequeue();

                for (int v = 0; v < Size; v++)
                {
                    if (/*IsNotCycle(u,v,parent) &&*/v != s && rGraph[u, v].RemainingFlow > 0)
                    {
                        if (parent[v] == null)
                        {
                            parent[v] = new MilestoneHist { pointNum = u, totalCost = parent[u].totalCost + rGraph[u, v].Cost };
                            q.Enqueue(v);
                        }
                        else
                        {
                            int newCost = parent[u].totalCost + rGraph[u, v].Cost;
                            int oldCost = parent[v].totalCost;
                            if (newCost < oldCost)
                            {
                                parent[v] = new MilestoneHist { pointNum = u, totalCost = parent[u].totalCost + rGraph[u, v].Cost };
                                q.Enqueue(v);
                            }
                        }
                    }
                }
            }
            // If we reached sink in BFS starting from source, then return
            // true, else false
            return parent;
        }

        public static bool IsNotCycle(int from, int to, MilestoneHist[] hist)
        {
            //code to determine if "from" is not a descendant of "to"
            throw new NotImplementedException();
        }

        public static Milestone[,] ConvertToMilestones(int[,] costs)
        {
            int size = costs.GetLength(0);
            Milestone[,] res = new Milestone[size, size];
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                {
                    res[i, j] = new Milestone { InitialFlow = costs[i, j], RemainingFlow = costs[i, j] };
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
        public int RemainingFlow { get; set; }
        public int Cost { get; set; }
        public Milestone prevMilestone { get; set; }

        public int getTotalCost()
        {
            var maxFlow = this.InitialFlow;
            var totalCost = 0;
            var prevMilestone = this.prevMilestone;
            while (prevMilestone != null)
            {
                maxFlow = Math.Min(maxFlow, prevMilestone.InitialFlow);
                prevMilestone = prevMilestone.prevMilestone;
            }
            prevMilestone = this.prevMilestone;
            while (prevMilestone != null)
            {
                totalCost += prevMilestone.Cost * maxFlow;
                prevMilestone = this.prevMilestone;
            }
            return totalCost;
        }
    }

    class MilestoneHist
    {
        public int pointNum { get; set; }
        public int totalCost { get; set; }

    }
}
