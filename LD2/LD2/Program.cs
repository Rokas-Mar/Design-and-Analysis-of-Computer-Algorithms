using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int n = 1;
            int[] arr = GenerateNewArray(n);
            int count = 0;
            var timer = new Stopwatch();

            timer.Start();
            methodToAnalysis(arr, ref count);
            FF2(arr, n);
            timer.Stop();

            Console.WriteLine(count);
            Console.WriteLine(timer.Elapsed);
        }

        public static int[] GenerateNewArray(int length)
        {
            int[] array = new int[length + 1];
            for (int i = 0; i < length; i++)
            {
                array[i] = i;
            }

            return array;
        }


        public static long methodToAnalysis(int[] arr, ref int count)
        {
            long n = arr.Length;
            count++;
            long k = n;
            count++;

            for (int i = 0; i < n; i++)
            {

                if (arr[i] / 7 == 0)
                {
                    k -= 2;
                    count++;
                }

                else
                {
                    k += 3;
                    count++;
                }
            }

            return k;
        }

        //T(n)=T(n−9)+ T(n−2)+n
        public static void FF2(int[] A, int k)
        {
            for (int i = 0; i < k; i++)
            {
            }

            if (k >= 9)
            {
                FF2(A, k - 9);
                FF2(A, k - 2);
            }
        }
    }
}
