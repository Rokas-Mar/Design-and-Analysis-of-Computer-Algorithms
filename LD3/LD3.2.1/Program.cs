using System;
using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        int numberOfBooks = 3;
        int[] pages = { 30, 15, 40, 10, 30 };
        DivideIntoBooks(pages, numberOfBooks);
    }

    static void DivideIntoBooks(int[] pagesPerChapter, int numberOfBooks)
    {
        int totalChapters = pagesPerChapter.Length;

        int totalPages = 0;
        foreach (int pages in pagesPerChapter)
        {
            totalPages += pages;
        }

        int[,] dp = new int[totalChapters + 1, numberOfBooks + 1];
        for (int i = 0; i <= totalChapters; i++)
        {
            for (int j = 0; j <= numberOfBooks; j++)
            {
                dp[i, j] = int.MaxValue;
            }
        }
        dp[0, 0] = 0;

        DivideIntoBooksRecursive(pagesPerChapter, totalChapters, numberOfBooks, dp);

        int[] distribution = new int[numberOfBooks];
        int remainingChapters = totalChapters;
        for (int i = numberOfBooks; i >= 1; i--)
        {
            int sum = 0;
            for (int j = remainingChapters; j >= 1; j--)
            {
                sum += pagesPerChapter[j - 1];
                if (dp[remainingChapters, i] == Math.Max(dp[j - 1, i - 1], sum))
                {
                    distribution[i - 1] = sum;
                    remainingChapters = j - 1;
                    break;
                }
            }
        }

        Console.WriteLine("Pages per book:");
        for (int i = 0; i < numberOfBooks; i++)
        {
            Console.WriteLine($"Book {i + 1}: {distribution[i]} pages");
        } 
    }

    static int DivideIntoBooksRecursive(int[] pagesPerChapter, int totalChapters, int numberOfBooks, int[,] dp)
    {
        if (totalChapters == 0)
            return 0;
        if (numberOfBooks == 0)
            return int.MaxValue;

        if (dp[totalChapters, numberOfBooks] != int.MaxValue)
            return dp[totalChapters, numberOfBooks];

        int sum = 0;
        int minMax = int.MaxValue;
        for (int i = totalChapters; i >= 1; i--)
        {
            sum += pagesPerChapter[i - 1];
            minMax = Math.Min(minMax, Math.Max(DivideIntoBooksRecursive(pagesPerChapter, i - 1, numberOfBooks - 1, dp), sum));
        }

        dp[totalChapters, numberOfBooks] = minMax;
        return minMax;
    }
}
