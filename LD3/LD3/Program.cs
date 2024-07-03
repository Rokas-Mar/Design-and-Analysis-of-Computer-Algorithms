using System;
using System.Diagnostics;

namespace LD3_1_1
{
    internal class Program
    {
        static int[,] board;

        static void Main(string[] args)
        {
            for (int m = 2; m <= 10; m++)
            {
                board = GenerateBoard(m, 10);
                MeasurePerformance(0, 0);
            }
        }
        static int MaxPoints(int x, int y)
        {
            int rows = board.GetLength(0);
            int cols = board.GetLength(1);

            if (x == rows - 1 && y == cols - 1)
                return board[x, y];

            int right = (y < cols - 1) ? MaxPoints(x, y + 1) : 0;
            int up = (x < rows - 1) ? MaxPoints(x + 1, y) : 0;
            int diagonal = (x < rows - 1 && y < cols - 1) ? MaxPoints(x + 1, y + 1) : 0;

            return board[x, y] + Math.Max(Math.Max(right, up), diagonal);
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

        static void MeasurePerformance(int startX, int startY)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            int maxPoints = MaxPoints(startX, startY);

            stopwatch.Stop();
            Console.WriteLine("Taskai:" + maxPoints);
            Console.WriteLine("laikas: " + stopwatch.Elapsed);
        }
    }
}
