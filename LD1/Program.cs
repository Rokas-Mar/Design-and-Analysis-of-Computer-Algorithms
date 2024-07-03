using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata;

namespace BMP_example
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Enter the width of the canvas:");
            //int width = Convert.ToInt32(Console.ReadLine());
            //Console.WriteLine("Enter the depth of the recursion:");
            //int depth = Convert.ToInt32(Console.ReadLine());
            //depth -= 1;
            //BMP(width, depth);

            //BMP(100, 5);
            //BMP(250, 5);
            //BMP(750, 5);
            //BMP(1000, 5);
            BMP(5000, 1000);
        }

        static void BMP(int width, int depth)
        {
            using (FileStream file = new FileStream("sample.bmp", FileMode.Create, FileAccess.Write))
            {
                int fileSize = CalculateFileSize(width, width);

                file.Write(
                    new byte[62]
                    {
                        // Header
                        0x42, 0x4d,
                        (byte)(fileSize & 0xff), (byte)((fileSize >> 8) & 0xff), (byte)((fileSize >> 16) & 0xff), (byte)((fileSize >> 24) & 0xff),
                        0x0, 0x0, 0x0, 0x0,
                        0x36, 0x0, 0x0, 0x0,
                        // Information header
                        0x28, 0x0, 0x0, 0x0,
                        (byte)(width & 0xff), (byte)((width >> 8) & 0xff), (byte)((width >> 16) & 0xff), (byte)((width >> 24) & 0xff),
                        (byte)(width & 0xff), (byte)((width >> 8) & 0xff), (byte)((width >> 16) & 0xff), (byte)((width >> 24) & 0xff),
                        0x1, 0x0,
                        0x1, 0x0,
                        0x0, 0x0, 0x0, 0x0,
                        0x0, 0x0, 0x0, 0x0,
                        0x0, 0x0, 0x0, 0x0,
                        0x0, 0x0, 0x0, 0x0,
                        0x0, 0x0, 0x0, 0x0,
                        0x0, 0x0, 0x0, 0x0,
                        // Color palette
                        0xff, 0xff, 0xff, 0x0,
                        0x0, 0x0, 0x0, 0x0
                    });

                int l = (width + 31) / 32 * 4;
                var t = new byte[width * l];

                int centerx = width / 2;
                int centery = width / 2;
                int size = width / 8;

                int count = 0;
                var timer = new Stopwatch();

                PrintDiamond(t, size, centerx, centery, width, l);
                timer.Start();
                CalculateDiamondCoordinates(t, centerx, centery, size * 7 / 3, size / 3, depth, l, width, ref count);
                timer.Stop();

                Console.WriteLine(count);
                Console.WriteLine(timer.Elapsed);

                file.Write(t);
                file.Close();

                new Process
                {
                    StartInfo = new ProcessStartInfo(@"sample.bmp")
                    {
                        UseShellExecute = true
                    }
                }.Start();
            }
        }

        static int CalculateFileSize(int width, int height)
        {
            int dataSize = ((width + 31) / 32) * 4 * height;
            return 54 + dataSize;
        }

        public static void DrawLine(byte[] t, int x0, int y0, int x1, int y1, int size, int l)
        {
            int dx = Math.Abs(x1 - x0);
            int dy = Math.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;
            int err = dx - dy;
            int x = x0;
            int y = y0;

            while (true)
            {
                if (x >= 0 && x < size && y >= 0 && y < size)
                {
                    int index = (y * l) + (x / 8);
                    t[index] |= (byte)(0x80 >> (x % 8));
                }

                if (x == x1 && y == y1)
                {
                    break;
                }

                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    x += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    y += sy;
                }
            }
        }

        public static void PrintDiamond(byte[] t, int size, int centerx, int centery, int width, int l)
        {
            DrawLine(t, centerx, centery - size, centerx + size, centery, width, l); // desine apacia
            DrawLine(t, centerx - size, centery, centerx, centery - size, width, l); ; // kaire apacia
            DrawLine(t, centerx - size, centery, centerx, centery + size, width, l); // kaire virsus
            DrawLine(t, centerx, centery + size, centerx + size, centery, width, l); ; // desine virsus

            FillColor(t, centerx, centery, 0, 1, width, l);
        }

        public static void FillColor(byte[] t, int x, int y, byte targetColor, byte replacementColor, int size, int l)
        {

            if (x < 0 || x >= size || y < 0 || y >= size)
                return;

            Stack<(int, int)> stack = new Stack<(int, int)>();
            stack.Push((x, y));

            while (stack.Count > 0)
            {
                (int cx, int cy) = stack.Pop();

                if (cx < 0 || cx >= size || cy < 0 || cy >= size)
                    continue;

                int index = (cy * l) + (cx / 8);
                int bitIndex = cx % 8;

                if (((t[index] >> (7 - bitIndex)) & 1) != targetColor)
                    continue;

                t[index] &= (byte)~(0x80 >> bitIndex); 
                t[index] |= (byte)((replacementColor & 1) << (7 - bitIndex));


                stack.Push((cx + 1, cy));
                stack.Push((cx - 1, cy));

                if (cy > 0)
                    stack.Push((cx, cy - 1));
                if (cy < size - 1)
                    stack.Push((cx, cy + 1));
            }
        }

        static void CalculateDiamondCoordinates(byte[] t, int centerX, int centerY, int distance, int size, int depth, int l, int width, ref int count)
        {
            if (depth == 0) return;
            if (size < 2) return;

            double theta = Math.PI / 10;
            count++;

            for (int n = 0; n < 5; n++)
            {
                int x = centerX + (int)(distance * Math.Cos(theta));
                count++;
                int y = centerY + (int)(distance * Math.Sin(theta));
                count++;
                theta += 2 * Math.PI / 5;
                count++;

                PrintDiamond(t, size, x, y, width, l);
                count++;
                CalculateDiamondCoordinates(t, x, y, distance * 19/50, (int)(size / 2.5), depth - 1, l, width, ref count);
            }
        }
    }
}
