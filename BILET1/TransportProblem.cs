using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BILET1
{
    public class TransportProblem
    {
        public double[] Supply { get; set; }
        public double[] Demand { get; set; }
        public double[,] Costs { get; set; }
        public double[,] Solution { get; set; }

        public bool IsBalanced { get; private set; }
        public double TotalCost { get; private set; }

        public void SolveByNorthWestCorner()
        {
            int m = Supply.Length;
            int n = Demand.Length;
            Solution = new double[m, n];

            double[] supplyRemaining = (double[])Supply.Clone();
            double[] demandRemaining = (double[])Demand.Clone();

            TotalCost = 0;

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (supplyRemaining[i] == 0 || demandRemaining[j] == 0)
                        continue;

                    double amount = Math.Min(supplyRemaining[i], demandRemaining[j]);
                    Solution[i, j] = amount;
                    TotalCost += amount * Costs[i, j];

                    supplyRemaining[i] -= amount;
                    demandRemaining[j] -= amount;

                    if (supplyRemaining[i] == 0)
                        break;
                }
            }
        }

        public void CheckBalanced()
        {
            double totalSupply = Supply.Sum();
            double totalDemand = Demand.Sum();
            IsBalanced = Math.Abs(totalSupply - totalDemand) < 0.0001;
        }
    }
}
