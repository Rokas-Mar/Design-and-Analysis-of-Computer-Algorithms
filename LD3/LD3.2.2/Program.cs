using System;
using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        int numberOfBooks = 3;
        int[] pagesPerChapter = { 30, 40, 5, 30 };

        int minPages = FindMinimumPages(pagesPerChapter, numberOfBooks);
        int[] booksPages = DivideIntoBooks(pagesPerChapter, numberOfBooks, minPages);

        Console.WriteLine("Expected result:");
        for (int i = 0; i < numberOfBooks; i++)
        {
            Console.WriteLine($"Book {i + 1}: {booksPages[i]} pages");
        }
    }

    static int FindMinimumPages(int[] pagesPerChapter, int numberOfBooks)
    {
        int left = 0;
        int right = 0;

        foreach (int pages in pagesPerChapter)
        {
            right += pages;
        }

        while (left < right)
        {
            int mid = left + (right - left) / 2;

            if (IsValid(pagesPerChapter, numberOfBooks, mid))
            {
                right = mid;
            }
            else
            {
                left = mid + 1;
            }
        }

        return left;
    }

    static bool IsValid(int[] pagesPerChapter, int numberOfBooks, int maxPages)
    {
        int booksCount = 1;
        int pagesInCurrentBook = 0;

        foreach (int pages in pagesPerChapter)
        {
            if (pagesInCurrentBook + pages <= maxPages)
            {
                pagesInCurrentBook += pages;
            }
            else
            {
                booksCount++;
                pagesInCurrentBook = pages;

                if (booksCount > numberOfBooks)
                {
                    return false;
                }
            }
        }

        return true;
    }

    static int[] DivideIntoBooks(int[] pagesPerChapter, int numberOfBooks, int minPages)
    {
        int[] booksPages = new int[numberOfBooks];
        int currentBookIndex = 0;
        int pagesInCurrentBook = 0;

        foreach (int pages in pagesPerChapter)
        {
            if (pagesInCurrentBook + pages <= minPages)
            {
                pagesInCurrentBook += pages;
            }
            else
            {
                booksPages[currentBookIndex++] = pagesInCurrentBook;
                pagesInCurrentBook = pages;
            }
        }

        booksPages[currentBookIndex] = pagesInCurrentBook;

        return booksPages;
    }
}
