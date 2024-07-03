using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD3_1_2
{
    internal class Program
    {
    static int[,] board;

        static int MaxPointsDynamic()
        {
            int rows = board.GetLength(0);
            int cols = board.GetLength(1);

            int[,] dp = new int[rows, cols];

            for (int i = rows - 1; i >= 0; i--)
            {
                for (int j = cols - 1; j >= 0; j--)
                {
                    if (i == rows - 1 && j == cols - 1)
                        dp[i, j] = board[i, j];
                    else
                    {
                        int right = (j < cols - 1) ? dp[i, j + 1] : 0;
                        int up = (i < rows - 1) ? dp[i + 1, j] : 0;
                        int diagonal = (i < rows - 1 && j < cols - 1) ? dp[i + 1, j + 1] : 0;

                        dp[i, j] = board[i, j] + Math.Max(Math.Max(right, up), diagonal);
                    }
                }
            }

            return dp[0, 0];
        }

        static void Main(string[] args)
        {
            for (int size = 10; size <= 100; size+=10)
            {
                board = GenerateBoard(size, 100);
                MeasurePerformance();
            }
        }

        static int[,] GenerateBoard(int m, int n)
        {
            int[,] newBoard = new int[m, n];
            Random random = new Random();

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    newBoard[i, j] = random.Next(1, 10);
                }
            }

            return newBoard;
        }

        static void MeasurePerformance()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            int maxPoints = MaxPointsDynamic();

            stopwatch.Stop();
            Console.WriteLine("Taskai:" + maxPoints);
            Console.WriteLine("laikas: " + stopwatch.Elapsed);
        }
    }
}