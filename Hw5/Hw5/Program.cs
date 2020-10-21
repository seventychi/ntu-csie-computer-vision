using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Hw5
{
    internal class Program
    {
        private class Point
        {
            public int X { get; set; }

            public int Y { get; set; }

            public int Value { get; set; }

            public Point(int x, int y, int value = 0)
            {
                X = x;
                Y = y;
                Value = value;
            }
        }

        private static readonly List<Point> Octagon = new List<Point>
        {
            new Point(-2, -1),
            new Point(-2, 0),
            new Point(-2, 1),
            new Point(-1, -2),
            new Point(-1, -1),
            new Point(-1, 0),
            new Point(-1, 1),
            new Point(-1, 2),
            new Point(0, -2),
            new Point(0, -1),
            new Point(0, 0),
            new Point(0, 1),
            new Point(0, 2),
            new Point(1, -2),
            new Point(1, -1),
            new Point(1, 0),
            new Point(1, 1),
            new Point(1, 2),
            new Point(2, -1),
            new Point(2, 0),
            new Point(2, 1)
        };

        private static void Main(string[] args)
        {
            try
            {
                Directory.CreateDirectory("answers");

                using var image = new Bitmap("lena.bmp");

                GetGrayDilationBitmap(image, Octagon)?.Save(@"answers\a.png");
                GetGrayErosionBitmap(image, Octagon)?.Save(@"answers\b.png");
                GetGrayOpeningBitmap(image, Octagon)?.Save(@"answers\c.png");
                GetGrayClosingBitmap(image, Octagon)?.Save(@"answers\d.png");

                Console.WriteLine("complete...");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 取得 Gray Dilation 影像
        /// </summary>
        /// <param name="srcImg">來源影像</param>
        /// <param name="kernel">Kernel</param>
        /// <returns>Gray Dilation Bitmap</returns>
        private static Bitmap GetGrayDilationBitmap(Bitmap srcImg, IList<Point> kernel)
        {
            var result = new Bitmap(srcImg);

            for (var x = 0; x < srcImg.Width; x++)
            {
                for (var y = 0; y < srcImg.Height; y++)
                {
                    var max = 0;

                    foreach (var point in kernel)
                    {
                        var px = x + point.X;
                        var py = y + point.Y;

                        if (px >= 0 && px < srcImg.Width && py >= 0 && py < srcImg.Height)
                        {
                            var tmp = srcImg.GetPixel(px, py).R + point.Value;

                            if (max < tmp)
                            {
                                max = tmp;
                            }
                        }
                    }

                    result.SetPixel(x, y, Color.FromArgb(max, max, max));
                }
            }

            return result;
        }

        /// <summary>
        /// 取得 Gray Erosion 影像
        /// </summary>
        /// <param name="srcImg">來源影像</param>
        /// <param name="kernel">Kernel</param>
        /// <returns>Gray Erosion Bitmap</returns>
        private static Bitmap GetGrayErosionBitmap(Bitmap srcImg, IList<Point> kernel)
        {
            var result = new Bitmap(srcImg);

            for (var x = 0; x < srcImg.Width; x++)
            {
                for (var y = 0; y < srcImg.Height; y++)
                {
                    var min = 256;

                    foreach (var point in kernel)
                    {
                        var px = x + point.X;
                        var py = y + point.Y;

                        if (px >= 0 && px < srcImg.Width && py >= 0 && py < srcImg.Height)
                        {
                            var tmp = srcImg.GetPixel(px, py).R - point.Value;
                            if (tmp < min)
                            {
                                min = tmp;
                            }
                        }
                    }

                    if (min < 0)
                    {
                        min = 0;
                    }

                    result.SetPixel(x, y, Color.FromArgb(min, min, min));
                }
            }

            return result;
        }

        /// <summary>
        /// 取得 Gray Opening 影像
        /// </summary>
        /// <param name="srcImg">來源影像</param>
        /// <param name="kernel">Kernel</param>
        /// <returns>Gray Opening Bitmap</returns>
        private static Bitmap GetGrayOpeningBitmap(Bitmap srcImg, IList<Point> kernel)
        {
            return GetGrayDilationBitmap(GetGrayErosionBitmap(srcImg, kernel), kernel);
        }

        /// <summary>
        /// 取得 Gray Closing 影像
        /// </summary>
        /// <param name="srcImg">來源影像</param>
        /// <param name="kernel">Kernel</param>
        /// <returns>Gray Closing Bitmap</returns>
        private static Bitmap GetGrayClosingBitmap(Bitmap srcImg, IList<Point> kernel)
        {
            return GetGrayErosionBitmap(GetGrayDilationBitmap(srcImg, kernel), kernel);
        }
    }
}