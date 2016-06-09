using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinCostFlow
{
    class Program
    {
        static void Main(string[] args)
        {
            //int neededFlow = 2;
            //int[,] flows = {
            //    {0,2,0,0,0,0 },
            //    {0,0,1,0,2,0 },
            //    {0,0,0,1,0,0 },
            //    {0,0,0,0,1,0 },
            //    {0,0,0,0,0,2 },
            //    {0,0,0,0,0,0 }
            //};
            //Array.Copy(flows, 0, flowsCopy, 0, flows.Length);
            //int[,] costs = {
            //    {0,1,0,0,0,0 },
            //    {0,0,1,0,6,0 },
            //    {0,0,0,1,0,0 },
            //    {0,0,0,0,1,0 },
            //    {0,0,0,0,0,1 },
            //    {0,0,0,0,0,0 }
            //};

            var data = getData();
            int[,] flowsCopy = new int[data.flows.GetLength(0), data.flows.GetLength(0)];
            Array.Copy(data.flows, 0, flowsCopy, 0, data.flows.Length);
            FlowCost result = CalculateFlow(data.flows, data.costs, 0, data.flows.GetLength(0) - 1, data.neededFlow);
            var usedFlow = getUsedFlow(flowsCopy, result.resultingFlows);
            writeResult(result.totalCost, usedFlow);
        }

        private static void writeResult(int totalCost, int[,] usedFlow, string source = "main")
        {
            string fullAppName = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string fullAppPath = System.IO.Path.GetDirectoryName(fullAppName);
            string root = System.IO.Path.GetDirectoryName(fullAppPath);
            root = System.IO.Path.GetDirectoryName(root);
            string DataPath = String.Concat(root, "\\Data", "\\" + source + "_result.csv");

            writeData(DataPath, totalCost, usedFlow);
        }

        private static void writeData(string dataPath, int totalCost, int[,] usedFlow)
        {
            StreamWriter sw = new StreamWriter(dataPath);
            sw.WriteLine(totalCost);
            sw.WriteLine("");
            for (int i = 0; i < usedFlow.GetLength(0); i++)
            {
                var line = "";
                for (int j = 0; j < usedFlow.GetLength(0); j++)
                {
                    line += usedFlow[i, j] + ",";
                }
                line = line.Trim(',');
                sw.WriteLine(line);
            }
            sw.Close();
        }

        private static dataReader getData(string source = "main")
        {
            string fullAppName = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string fullAppPath = System.IO.Path.GetDirectoryName(fullAppName);
            string root = System.IO.Path.GetDirectoryName(fullAppPath);
            root = System.IO.Path.GetDirectoryName(root);
            string DataPath = String.Concat(root, "\\Data", "\\" + source + ".csv");

            if (File.Exists(DataPath))
                return readData(DataPath);
            else
                throw new FileNotFoundException(); ;
        }

        private static dataReader readData(string filePath)
        {
            var result = new dataReader();
            //int[,] flows;
            //int[,] costs;
            try
            {
                StreamReader sr = new StreamReader(filePath);

                string line;

                line = sr.ReadLine();
                result.neededFlow = System.Convert.ToInt32(line.Trim(','));
                line = sr.ReadLine();
                result.flows = new int[line.Split(',').Length, line.Split(',').Length];
                result.costs = new int[line.Split(',').Length, line.Split(',').Length];
                var lineNum = 0;
                while (line.Length > 2)
                {
                    string[] LimitersStr = line.Split(',');
                    int len = LimitersStr.Length;
                    for (int i = 0; i < len; i++)
                    {
                        result.flows[lineNum, i] = System.Convert.ToInt32(LimitersStr[i]);
                    }
                    lineNum++;
                    line = sr.ReadLine();
                }
                lineNum = 0;
                while (sr.Peek() > 0)
                {
                    line = sr.ReadLine();
                    string[] LimitersStr = line.Split(',');
                    int len = LimitersStr.Length;
                    for (int i = 0; i < len; i++)
                    {
                        result.costs[lineNum, i] = System.Convert.ToInt32(LimitersStr[i]);
                    }
                    lineNum++;
                }
                sr.Close();
                //result.flows = flows;
                //result.costs = costs;
                return result;
            }
            catch (Exception)
            {
                return readData("main");
            }
        }

        private static int[,] getUsedFlow(int[,] flows, int[,] resultingFlows)
        {
            var Size = flows.GetLength(0);
            int[,] result = new int[Size, Size];
            for (int i = 0; i < Size; i++)
                for (int j = 0; j < Size; j++)
                {
                    result[i, j] = flows[i, j] - resultingFlows[i, j];
                }
            return result;
        }

        private static FlowCost CalculateFlow(int[,] flows, int[,] costs, int s, int t, int neededFlow)
        {
            var totalFlow = 0;
            var totalCost = 0;
            if (s == t)
                throw new ArgumentException("input and output should be different points");
            MilestoneHist[] path = bfs(flows, costs, s, t);
            while (path[t] != null)
            {
                int maxFlow = getMaxFlowForPath(flows, path, s, t);
                if (neededFlow - totalFlow < maxFlow)
                {
                    maxFlow = neededFlow - totalFlow;
                    flows = updateFlows(maxFlow, flows, path, s, t);
                    totalCost += maxFlow * path[t].totalCost;
                    path = bfs(flows, costs, s, t);
                    totalFlow += maxFlow;
                    return new FlowCost { totalCost = totalCost, resultingFlows = flows };
                }
                flows = updateFlows(maxFlow, flows, path, s, t);
                totalCost += maxFlow * path[t].totalCost;
                path = bfs(flows, costs, s, t);
                totalFlow += maxFlow;
            }
            return new FlowCost { totalCost = totalCost, resultingFlows = flows };
        }

        private static int[,] updateFlows(int maxFlow, int[,] flows, MilestoneHist[] path, int s, int t)
        {
            int to = t;
            int from = path[to].pointNum;
            while (from != -1)
            {
                flows[from, to] -= maxFlow;
                if (flows[from, to] < 0)
                    throw new Exception("result flow can't be less than 0, some error in logic");
                to = from;
                from = path[to].pointNum;
            }
            return flows;
        }

        private static int getMaxFlowForPath(int[,] flows, MilestoneHist[] path, int s, int t)
        {
            int to = t;
            int from = path[to].pointNum;
            var maxFlow = flows[from, to];
            while (from != -1)
            {
                maxFlow = Math.Min(maxFlow, flows[from, to]);
                to = from;
                from = path[to].pointNum;
            }
            return maxFlow;
        }

        public static MilestoneHist[] bfs(int[,] rGraph, int[,] costGraph, int s, int t)
        {
            var debug = 0;
            int Size = rGraph.GetLength(0);
            MilestoneHist[] parent = new MilestoneHist[Size];
            parent[s] = new MilestoneHist { totalCost = 0, pointNum = -1 };
            bool[] visited = new bool[Size];

            Queue q = new Queue();
            q.Enqueue(s);

            while (q.Count != 0)
            {
                int u = (int)q.Dequeue();
                debug++;
                if (debug > 10000)
                    throw new StackOverflowException();

                for (int v = 0; v < Size; v++)
                {
                    if (v != s && rGraph[u, v] > 0 /*&& IsNotCycle(u,v,parent)*/)
                    {
                        if (parent[v] == null)
                        {
                            parent[v] = new MilestoneHist { pointNum = u, totalCost = parent[u].totalCost + costGraph[u, v] };
                            q.Enqueue(v);
                        }
                        else
                        {
                            int newCost = parent[u].totalCost + costGraph[u, v];
                            int oldCost = parent[v].totalCost;
                            if (newCost < oldCost)
                            {
                                parent[v] = new MilestoneHist { pointNum = u, totalCost = newCost };
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

        public static int GetCostByHistory(MilestoneHist[] history)
        {
            throw new NotImplementedException();
        }
    }

    class FlowCost
    {
        public int totalFlow { get; set; }
        public int totalCost { get; set; }
        public int[,] resultingFlows { get; set; }
    }
    class dataReader
    {
        public int neededFlow { get; set; }
        public int[,] flows { get; set; }
        public int[,] costs { get; set; }
    }
}
