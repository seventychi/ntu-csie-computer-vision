using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Hw6
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                Directory.CreateDirectory("answers");

                using var image = new Bitmap("lena.bmp");

                var binaryBitmap = GetBinaryBitmap(image);
                var downSamplingBitmap = GetDownSamplingBitmap(binaryBitmap);
                var yokoiConnectivityNumberMatrix = GetYokoiConnectivityNumberMatrix(downSamplingBitmap);

                var lines = new List<string>();

                for (var y = 0; y < 64; y++)
                {
                    var line = "";

                    for (var x = 0; x < 64; x++)
                    {
                        var number = yokoiConnectivityNumberMatrix[x, y];
                        line += $"{(number == 0 ? " " : number.ToString())} ";
                    }

                    lines.Add(line);
                }

                File.WriteAllLines(@"answers\\a.txt", lines);

                Console.WriteLine("completed...");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadKey();
        }

        /// <summary>
        /// 取得 Yokoi Connectivity Number Matrix
        /// </summary>
        private static int[,] GetYokoiConnectivityNumberMatrix(Bitmap srcImg)
        {
            var result = new int[64, 64];

            for (var x = 0; x < srcImg.Width; x++)
            {
                for (var y = 0; y < srcImg.Height; y++)
                {
                    var center = srcImg.GetPixel(x, y).R;
                    if (center == 0)
                    {
                        result[x, y] = 0;
                        continue;
                    }

                    // a1
                    var b = center;
                    var c = x < srcImg.Width - 1 ? srcImg.GetPixel(x + 1, y).R : 0;
                    var d = x < srcImg.Width - 1 && y >= 1 ? srcImg.GetPixel(x + 1, y - 1).R : 0;
                    var e = y >= 1 ? srcImg.GetPixel(x, y - 1).R : 0;

                    var a1 = H(b, c, d, e);

                    // a2
                    c = e;
                    d = x >= 1 && y >= 1 ? srcImg.GetPixel(x - 1, y - 1).R : 0;
                    e = x >= 1 ? srcImg.GetPixel(x - 1, y).R : 0;

                    var a2 = H(b, c, d, e);

                    // a3
                    c = e;
                    d = x >= 1 && y < srcImg.Height - 1 ? srcImg.GetPixel(x - 1, y + 1).R : 0;
                    e = y < srcImg.Height - 1 ? srcImg.GetPixel(x, y + 1).R : 0;

                    var a3 = H(b, c, d, e);

                    // a4
                    c = e;
                    d = x < srcImg.Width - 1 && y < srcImg.Height - 1 ? srcImg.GetPixel(x + 1, y + 1).R : 0;
                    e = x < srcImg.Width - 1 ? srcImg.GetPixel(x + 1, y).R : 0;

                    var a4 = H(b, c, d, e);
                    result[x, y] = F(a1, a2, a3, a4);
                }
            }

            return result;
        }

        /// <summary>
        /// 根據 h(b, c, d, e) 的算式求得 q, r, s
        /// </summary>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private static char H(int b, int c, int d, int e)
        {
            // q if b == c and (d != b || e != b)
            if (b == c && (d != b || e != b))
                return 'q';
            // r if b == c and (d == b && e == b)
            if (b == c && d == b && e == b)
                return 'r';
            // b != c
            return 's';
        }

        /// <summary>
        /// 根據 f(a1, a2, a3, a4) 的算式求得 number
        /// </summary>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <param name="a3"></param>
        /// <param name="a4"></param>
        /// <returns></returns>
        private static int F(char a1, char a2, char a3, char a4)
        {
            var count = 0;

            // 5 if a1 == a2 == a3 == a4 == r
            if (a1 == 'r' && a2 == 'r' && a3 == 'r' && a4 == 'r')
                return 5;

            // n where number of q
            if (a1 == 'q') count += 1;
            if (a2 == 'q') count += 1;
            if (a3 == 'q') count += 1;
            if (a4 == 'q') count += 1;

            return count;
        }

        /// <summary>
        /// 取得 Binary Image (Threshold at 128)
        /// </summary>
        /// <param name="srcImg">來源影像</param>
        /// <returns>二位元影像</returns>
        private static Bitmap GetBinaryBitmap(Bitmap srcImg)
        {
            if (srcImg == null)
                throw new Exception("source image is null");

            var dstBmp = new Bitmap(srcImg.Width, srcImg.Height);

            // 兩層迴圈取得所有 pixel 值(x, y)
            for (var x = 0; x < srcImg.Width; x++)
                for (var y = 0; y < srcImg.Height; y++)
                {
                    // 取得 Pixel RGB 值, 除以 3 後得到灰階值
                    var color = srcImg.GetPixel(x, y);
                    var gray = (color.R + color.G + color.B) / 3;

                    // 值 >= 128 轉為白色(255, 255, 255), 否則轉為黑色(0, 0, 0)
                    dstBmp.SetPixel(x, y, gray >= 128 ? Color.White : Color.Black);
                }

            return dstBmp;
        }

        /// <summary>
        /// 取得 down size 的影像 (64 * 64)
        /// </summary>
        /// <param name="srcImg"></param>
        /// <returns></returns>
        private static Bitmap GetDownSamplingBitmap(Bitmap srcImg)
        {
            var dstImg = new Bitmap(64, 64);

            if (srcImg == null)
                throw new Exception("source image is null");

            for (var x = 0; x < srcImg.Width; x += 8)
            {
                for (var y = 0; y < srcImg.Height; y += 8)
                {
                    dstImg.SetPixel(x / 8, y / 8, srcImg.GetPixel(x, y));
                }
            }

            return dstImg;
        }
    }
}