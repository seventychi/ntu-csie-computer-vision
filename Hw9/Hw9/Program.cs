using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Hw9
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                Directory.CreateDirectory("answers");

                using var image = new Bitmap("lena.bmp");

                GetRobertsImage(image, 12)?.Save(@"answers\a.png");
                GetPrewittImage(image, 24)?.Save(@"answers\b.png");
                GetSobelImage(image, 38)?.Save(@"answers\c.png");
                GetFreiChenImage(image, 30)?.Save(@"answers\d.png");
                GetKirschImage(image, 135)?.Save(@"answers\e.png");
                GetRobinsonImge(image, 43)?.Save(@"answers\f.png");
                GetNevatiaBabuImage(image, 12500)?.Save(@"answers\g.png");

                Console.WriteLine("complete...");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 取得 Roberts Image
        /// </summary>
        /// <param name="srcImg"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        private static Bitmap GetRobertsImage(Bitmap srcImg, int threshold)
        {
            var result = new Bitmap(srcImg);

            for (var x = 0; x < srcImg.Width; x++)
            {
                for (var y = 0; y < srcImg.Height; y++)
                {
                    // 取得 (x0, y0), (x1, y1) 及 (x1, y0), (x0, y1)
                    var x0 = x;
                    var y0 = y;
                    var x1 = x + 1 < srcImg.Width - 1 ? x + 1 : srcImg.Width - 1;
                    var y1 = y + 1 < srcImg.Height - 1 ? y + 1 : srcImg.Height - 1;

                    // 計算 r1 及 r2
                    var r1 = -srcImg.GetPixel(x0, y0).R + srcImg.GetPixel(x1, y1).R;
                    var r2 = -srcImg.GetPixel(x1, y0).R + srcImg.GetPixel(x0, y1).R;

                    // 藉由公式取得 gradient magnitude，並藉由 magnitude 與 threshold 比較 pixel 的結果
                    var magnitude = Convert.ToInt32(Math.Sqrt(Math.Pow(r1, 2) + Math.Pow(r2, 2)));
                    result.SetPixel(x, y, magnitude >= threshold ? Color.Black : Color.White);
                }
            }

            return result;
        }

        /// <summary>
        /// 取得 Prewitt Image
        /// </summary>
        /// <param name="srcImg"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        private static Bitmap GetPrewittImage(Bitmap srcImg, int threshold)
        {
            var result = new Bitmap(srcImg);

            for (var x = 0; x < srcImg.Width; x++)
            {
                for (var y = 0; y < srcImg.Height; y++)
                {
                    // 取得橫向 (x0, y2), (x1, y2), (x2, y2) 及 (x0, y0), (x1, y0), (x2, y0)
                    // 及縱向 (x2, y0), (x2, y1), (x2, y2) 及 (x0, y0), (x0, y1), (x0, y2)
                    var x0 = Math.Max(x - 1, 0);
                    var y0 = Math.Max(y - 1, 0);
                    var x1 = x;
                    var y1 = y;
                    var x2 = Math.Min(x + 1, srcImg.Width - 1);
                    var y2 = Math.Min(y + 1, srcImg.Height - 1);

                    // 計算 p1, p2
                    var p1 = (srcImg.GetPixel(x0, y2).R + srcImg.GetPixel(x1, y2).R + srcImg.GetPixel(x2, y2).R) -
                             (srcImg.GetPixel(x0, y0).R + srcImg.GetPixel(x1, y0).R + srcImg.GetPixel(x2, y0).R);
                    var p2 = (srcImg.GetPixel(x2, y0).R + srcImg.GetPixel(x2, y1).R + srcImg.GetPixel(x2, y2).R) -
                             (srcImg.GetPixel(x0, y0).R + srcImg.GetPixel(x0, y1).R + srcImg.GetPixel(x0, y2).R);

                    // 藉由公式取得 gradient magnitude，並藉由 magnitude 與 threshold 比較 pixel 的結果
                    var magnitude = Convert.ToInt32(Math.Sqrt(Math.Pow(p1, 2) + Math.Pow(p2, 2)));
                    result.SetPixel(x, y, magnitude >= threshold ? Color.Black : Color.White);
                }
            }

            return result;
        }

        /// <summary>
        /// 取得 Sobel Image
        /// </summary>
        /// <param name="srcImg"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        private static Bitmap GetSobelImage(Bitmap srcImg, int threshold)
        {
            var result = new Bitmap(srcImg);

            for (var x = 0; x < srcImg.Width; x++)
            {
                for (var y = 0; y < srcImg.Height; y++)
                {
                    // 取得橫向 (x0, y2), (x1, y2), (x2, y2) 及 (x0, y0), (x1, y0), (x2, y0)
                    // 及縱向 (x2, y0), (x2, y1), (x2, y2) 及 (x0, y0), (x0, y1), (x0, y2)
                    var x0 = Math.Max(x - 1, 0);
                    var y0 = Math.Max(y - 1, 0);
                    var x1 = x;
                    var y1 = y;
                    var x2 = Math.Min(x + 1, srcImg.Width - 1);
                    var y2 = Math.Min(y + 1, srcImg.Height - 1);

                    // 計算 s1, s2
                    var s1 = (srcImg.GetPixel(x0, y2).R + 2 * srcImg.GetPixel(x1, y2).R + srcImg.GetPixel(x2, y2).R) -
                             (srcImg.GetPixel(x0, y0).R + 2 * srcImg.GetPixel(x1, y0).R + srcImg.GetPixel(x2, y0).R);
                    var s2 = (srcImg.GetPixel(x2, y0).R + 2 * srcImg.GetPixel(x2, y1).R + srcImg.GetPixel(x2, y2).R) -
                             (srcImg.GetPixel(x0, y0).R + 2 * srcImg.GetPixel(x0, y1).R + srcImg.GetPixel(x0, y2).R);

                    // 藉由公式取得 gradient magnitude，並藉由 magnitude 與 threshold 比較 pixel 的結果
                    var magnitude = Convert.ToInt32(Math.Sqrt(Math.Pow(s1, 2) + Math.Pow(s2, 2)));
                    result.SetPixel(x, y, magnitude >= threshold ? Color.Black : Color.White);
                }
            }

            return result;
        }

        /// <summary>
        /// 取得 FreiChen Image
        /// </summary>
        /// <param name="srcImg"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        private static Bitmap GetFreiChenImage(Bitmap srcImg, int threshold)
        {
            var result = new Bitmap(srcImg);

            for (var x = 0; x < srcImg.Width; x++)
            {
                for (var y = 0; y < srcImg.Height; y++)
                {
                    // 取得橫向 (x0, y2), (x1, y2), (x2, y2) 及 (x0, y0), (x1, y0), (x2, y0)
                    // 及縱向 (x2, y0), (x2, y1), (x2, y2) 及 (x0, y0), (x0, y1), (x0, y2)
                    var x0 = Math.Max(x - 1, 0);
                    var y0 = Math.Max(y - 1, 0);
                    var x1 = x;
                    var y1 = y;
                    var x2 = Math.Min(x + 1, srcImg.Width - 1);
                    var y2 = Math.Min(y + 1, srcImg.Height - 1);

                    // 計算 f1, f2
                    var f1 = (srcImg.GetPixel(x0, y2).R + Math.Sqrt(2) * srcImg.GetPixel(x1, y2).R +
                              srcImg.GetPixel(x2, y2).R) -
                             (srcImg.GetPixel(x0, y0).R + Math.Sqrt(2) * srcImg.GetPixel(x1, y0).R +
                              srcImg.GetPixel(x2, y0).R);
                    var f2 = (srcImg.GetPixel(x2, y0).R + Math.Sqrt(2) * srcImg.GetPixel(x2, y1).R +
                              srcImg.GetPixel(x2, y2).R) -
                             (srcImg.GetPixel(x0, y0).R + Math.Sqrt(2) * srcImg.GetPixel(x0, y1).R +
                              srcImg.GetPixel(x0, y2).R);

                    // 藉由公式取得 gradient magnitude，並藉由 magnitude 與 threshold 比較 pixel 的結果
                    var magnitude = Convert.ToInt32(Math.Sqrt(Math.Pow(f1, 2) + Math.Pow(f2, 2)));
                    result.SetPixel(x, y, magnitude >= threshold ? Color.Black : Color.White);
                }
            }

            return result;
        }

        /// <summary>
        /// 取得 Kirsch Image
        /// </summary>
        /// <param name="srcImg"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        private static Bitmap GetKirschImage(Bitmap srcImg, int threshold)
        {
            var result = new Bitmap(srcImg);

            for (var x = 0; x < srcImg.Width; x++)
            {
                for (var y = 0; y < srcImg.Height; y++)
                {
                    // 取得相對應的 x, y 值
                    var x0 = Math.Max(x - 1, 0);
                    var y0 = Math.Max(y - 1, 0);
                    var x1 = x;
                    var y1 = y;
                    var x2 = Math.Min(x + 1, srcImg.Width - 1);
                    var y2 = Math.Min(y + 1, srcImg.Height - 1);

                    var kList = new List<int>();

                    // 計算 k0 ~ k8 的值
                    var k0 = -3 * srcImg.GetPixel(x0, y0).R - 3 * srcImg.GetPixel(x1, y0).R +
                             5 * srcImg.GetPixel(x2, y0).R
                             - 3 * srcImg.GetPixel(x0, y1).R + 5 * srcImg.GetPixel(x2, y1).R
                             - 3 * srcImg.GetPixel(x0, y2).R - 3 * srcImg.GetPixel(x1, y2).R +
                             5 * srcImg.GetPixel(x2, y2).R;
                    var k1 = -3 * srcImg.GetPixel(x0, y0).R + 5 * srcImg.GetPixel(x1, y0).R +
                             5 * srcImg.GetPixel(x2, y0).R
                             - 3 * srcImg.GetPixel(x0, y1).R + 5 * srcImg.GetPixel(x2, y1).R
                             - 3 * srcImg.GetPixel(x0, y2).R - 3 * srcImg.GetPixel(x1, y2).R -
                             3 * srcImg.GetPixel(x2, y2).R;
                    var k2 = 5 * srcImg.GetPixel(x0, y0).R + 5 * srcImg.GetPixel(x1, y0).R +
                             5 * srcImg.GetPixel(x2, y0).R
                             - 3 * srcImg.GetPixel(x0, y1).R - 3 * srcImg.GetPixel(x2, y1).R
                             - 3 * srcImg.GetPixel(x0, y2).R - 3 * srcImg.GetPixel(x1, y2).R -
                             3 * srcImg.GetPixel(x2, y2).R;
                    var k3 = 5 * srcImg.GetPixel(x0, y0).R + 5 * srcImg.GetPixel(x1, y0).R -
                        3 * srcImg.GetPixel(x2, y0).R
                        + 5 * srcImg.GetPixel(x0, y1).R - 3 * srcImg.GetPixel(x2, y1).R
                                                        - 3 * srcImg.GetPixel(x0, y2).R -
                                                        3 * srcImg.GetPixel(x1, y2).R -
                                                        3 * srcImg.GetPixel(x2, y2).R;
                    var k4 = 5 * srcImg.GetPixel(x0, y0).R - 3 * srcImg.GetPixel(x1, y0).R -
                             3 * srcImg.GetPixel(x2, y0).R
                             + 5 * srcImg.GetPixel(x0, y1).R - 3 * srcImg.GetPixel(x2, y1).R
                             + 5 * srcImg.GetPixel(x0, y2).R - 3 * srcImg.GetPixel(x1, y2).R -
                             3 * srcImg.GetPixel(x2, y2).R;
                    var k5 = -3 * srcImg.GetPixel(x0, y0).R - 3 * srcImg.GetPixel(x1, y0).R -
                             3 * srcImg.GetPixel(x2, y0).R
                             + 5 * srcImg.GetPixel(x0, y1).R - 3 * srcImg.GetPixel(x2, y1).R
                             + 5 * srcImg.GetPixel(x0, y2).R + 5 * srcImg.GetPixel(x1, y2).R -
                             3 * srcImg.GetPixel(x2, y2).R;
                    var k6 = -3 * srcImg.GetPixel(x0, y0).R - 3 * srcImg.GetPixel(x1, y0).R -
                             3 * srcImg.GetPixel(x2, y0).R
                             - 3 * srcImg.GetPixel(x0, y1).R - 3 * srcImg.GetPixel(x2, y1).R
                             + 5 * srcImg.GetPixel(x0, y2).R + 5 * srcImg.GetPixel(x1, y2).R +
                             5 * srcImg.GetPixel(x2, y2).R;
                    var k7 = -3 * srcImg.GetPixel(x0, y0).R - 3 * srcImg.GetPixel(x1, y0).R -
                             3 * srcImg.GetPixel(x2, y0).R
                             - 3 * srcImg.GetPixel(x0, y1).R + 5 * srcImg.GetPixel(x2, y1).R
                             - 3 * srcImg.GetPixel(x0, y2).R + 5 * srcImg.GetPixel(x1, y2).R +
                             5 * srcImg.GetPixel(x2, y2).R;

                    kList.Add(k0);
                    kList.Add(k1);
                    kList.Add(k2);
                    kList.Add(k3);
                    kList.Add(k4);
                    kList.Add(k5);
                    kList.Add(k6);
                    kList.Add(k7);

                    // 取得 k0 ~ k7 的最大值
                    var magnitude = kList.Max();
                    result.SetPixel(x, y, magnitude >= threshold ? Color.Black : Color.White);
                }
            }

            return result;
        }

        /// <summary>
        /// 取得 RobinsonImage
        /// </summary>
        /// <param name="srcImg"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        private static Bitmap GetRobinsonImge(Bitmap srcImg, int threshold)
        {
            var result = new Bitmap(srcImg);

            for (var x = 0; x < srcImg.Width; x++)
            {
                for (var y = 0; y < srcImg.Height; y++)
                {
                    // 取得相對應的 x, y 值
                    var x0 = Math.Max(x - 1, 0);
                    var y0 = Math.Max(y - 1, 0);
                    var x1 = x;
                    var y1 = y;
                    var x2 = Math.Min(x + 1, srcImg.Width - 1);
                    var y2 = Math.Min(y + 1, srcImg.Height - 1);

                    var rList = new List<int>();

                    var r0 = -1 * srcImg.GetPixel(x0, y0).R - 2 * srcImg.GetPixel(x0, y1).R -
                             1 * srcImg.GetPixel(x0, y2).R
                             + 1 * srcImg.GetPixel(x2, y0).R + 2 * srcImg.GetPixel(x2, y1).R +
                             1 * srcImg.GetPixel(x2, y2).R;
                    var r1 = -1 * srcImg.GetPixel(x0, y1).R - 2 * srcImg.GetPixel(x0, y2).R -
                             1 * srcImg.GetPixel(x1, y2).R
                             + 1 * srcImg.GetPixel(x1, y0).R + 2 * srcImg.GetPixel(x2, y0).R +
                             1 * srcImg.GetPixel(x2, y1).R;
                    var r2 = -1 * srcImg.GetPixel(x0, y2).R - 2 * srcImg.GetPixel(x1, y2).R -
                             1 * srcImg.GetPixel(x2, y2).R
                             + 1 * srcImg.GetPixel(x0, y0).R + 2 * srcImg.GetPixel(x1, y0).R +
                             1 * srcImg.GetPixel(x2, y0).R;
                    var r3 = -1 * srcImg.GetPixel(x1, y2).R - 2 * srcImg.GetPixel(x2, y2).R -
                             1 * srcImg.GetPixel(x2, y1).R
                             + 1 * srcImg.GetPixel(x0, y1).R + 2 * srcImg.GetPixel(x0, y0).R +
                             1 * srcImg.GetPixel(x1, y0).R;
                    var r4 = -1 * srcImg.GetPixel(x2, y0).R - 2 * srcImg.GetPixel(x2, y1).R -
                             1 * srcImg.GetPixel(x2, y2).R
                             + 1 * srcImg.GetPixel(x0, y0).R + 2 * srcImg.GetPixel(x0, y1).R +
                             1 * srcImg.GetPixel(x0, y2).R;
                    var r5 = -1 * srcImg.GetPixel(x1, y0).R - 2 * srcImg.GetPixel(x2, y0).R -
                             1 * srcImg.GetPixel(x2, y1).R
                             + 1 * srcImg.GetPixel(x0, y1).R + 2 * srcImg.GetPixel(x0, y2).R +
                             1 * srcImg.GetPixel(x1, y2).R;
                    var r6 = -1 * srcImg.GetPixel(x0, y0).R - 2 * srcImg.GetPixel(x1, y0).R -
                             1 * srcImg.GetPixel(x2, y0).R
                             + 1 * srcImg.GetPixel(x0, y2).R + 2 * srcImg.GetPixel(x1, y2).R +
                             1 * srcImg.GetPixel(x2, y2).R;
                    var r7 = -1 * srcImg.GetPixel(x0, y1).R - 2 * srcImg.GetPixel(x0, y0).R -
                             1 * srcImg.GetPixel(x1, y0).R
                             + 1 * srcImg.GetPixel(x1, y2).R + 2 * srcImg.GetPixel(x2, y2).R +
                             1 * srcImg.GetPixel(x2, y1).R;

                    rList.Add(r0);
                    rList.Add(r1);
                    rList.Add(r2);
                    rList.Add(r3);
                    rList.Add(r4);
                    rList.Add(r5);
                    rList.Add(r6);
                    rList.Add(r7);

                    // 取得 r0 ~ r7 的最大值
                    var magnitude = rList.Max();
                    result.SetPixel(x, y, magnitude >= threshold ? Color.Black : Color.White);
                }
            }

            return result;
        }

        /// <summary>
        /// 取得 NevatiaBabu Image
        /// </summary>
        /// <param name="srcImg"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        private static Bitmap GetNevatiaBabuImage(Bitmap srcImg, int threshold)
        {
            var result = new Bitmap(srcImg);

            for (var x = 0; x < srcImg.Width; x++)
            {
                for (var y = 0; y < srcImg.Height; y++)
                {
                    // 取得相關的 x, y
                    var x0 = Math.Max(x - 2, 0);
                    var y0 = Math.Max(y - 2, 0);
                    var x1 = Math.Max(x - 1, 0);
                    var y1 = Math.Max(y - 1, 0);
                    var x2 = x;
                    var y2 = y;
                    var x3 = Math.Min(x + 1, srcImg.Width - 1);
                    var y3 = Math.Min(y + 1, srcImg.Height - 1);
                    var x4 = Math.Min(x + 2, srcImg.Width - 1);
                    var y4 = Math.Min(y + 2, srcImg.Height - 1);

                    var neighbors = new int[]
                    {
                        srcImg.GetPixel(x0, y0).R,
                        srcImg.GetPixel(x1, y0).R,
                        srcImg.GetPixel(x2, y0).R,
                        srcImg.GetPixel(x3, y0).R,
                        srcImg.GetPixel(x4, y0).R,
                        srcImg.GetPixel(x0, y1).R,
                        srcImg.GetPixel(x1, y1).R,
                        srcImg.GetPixel(x2, y1).R,
                        srcImg.GetPixel(x3, y1).R,
                        srcImg.GetPixel(x4, y1).R,
                        srcImg.GetPixel(x0, y2).R,
                        srcImg.GetPixel(x1, y2).R,
                        srcImg.GetPixel(x2, y2).R,
                        srcImg.GetPixel(x3, y2).R,
                        srcImg.GetPixel(x4, y2).R,
                        srcImg.GetPixel(x0, y3).R,
                        srcImg.GetPixel(x1, y3).R,
                        srcImg.GetPixel(x2, y3).R,
                        srcImg.GetPixel(x3, y3).R,
                        srcImg.GetPixel(x4, y3).R,
                        srcImg.GetPixel(x0, y4).R,
                        srcImg.GetPixel(x1, y4).R,
                        srcImg.GetPixel(x2, y4).R,
                        srcImg.GetPixel(x3, y4).R,
                        srcImg.GetPixel(x4, y4).R
                    };

                    var nList = new List<int>();

                    // 根據公式計算 n0 ~ n5
                    var n0 = (100) * neighbors[0] + (100) * neighbors[1] + (100) * neighbors[2] + (100) * neighbors[3] +
                             (100) * neighbors[4] +
                             (100) * neighbors[5] + (100) * neighbors[6] + (100) * neighbors[7] + (100) * neighbors[8] +
                             (100) * neighbors[9] +
                             (0) * neighbors[10] + (0) * neighbors[11] + (0) * neighbors[12] + (0) * neighbors[13] +
                             (0) * neighbors[14] +
                             (-100) * neighbors[15] + (-100) * neighbors[16] + (-100) * neighbors[17] +
                             (-100) * neighbors[18] + (-100) * neighbors[19] +
                             (-100) * neighbors[20] + (-100) * neighbors[21] + (-100) * neighbors[22] +
                             (-100) * neighbors[23] + (-100) * neighbors[24];
                    var n1 = (100) * neighbors[0] + (100) * neighbors[1] + (100) * neighbors[2] + (100) * neighbors[3] +
                             (100) * neighbors[4] +
                             (100) * neighbors[5] + (100) * neighbors[6] + (100) * neighbors[7] + (78) * neighbors[8] +
                             (-32) * neighbors[9] +
                             (100) * neighbors[10] + (92) * neighbors[11] + (0) * neighbors[12] +
                             (-92) * neighbors[13] + (-100) * neighbors[14] +
                             (32) * neighbors[15] + (-78) * neighbors[16] + (-100) * neighbors[17] +
                             (-100) * neighbors[18] + (-100) * neighbors[19] +
                             (-100) * neighbors[20] + (-100) * neighbors[21] + (-100) * neighbors[22] +
                             (-100) * neighbors[23] + (-100) * neighbors[24];
                    var n2 = (100) * neighbors[0] + (100) * neighbors[1] + (100) * neighbors[2] + (32) * neighbors[3] +
                             (-100) * neighbors[4] +
                             (100) * neighbors[5] + (100) * neighbors[6] + (92) * neighbors[7] + (-78) * neighbors[8] +
                             (-100) * neighbors[9] +
                             (100) * neighbors[10] + (100) * neighbors[11] + (0) * neighbors[12] +
                             (-100) * neighbors[13] + (-100) * neighbors[14] +
                             (100) * neighbors[15] + (78) * neighbors[16] + (-92) * neighbors[17] +
                             (-100) * neighbors[18] + (-100) * neighbors[19] +
                             (100) * neighbors[20] + (-32) * neighbors[21] + (-100) * neighbors[22] +
                             (-100) * neighbors[23] + (-100) * neighbors[24];
                    var n3 = (-100) * neighbors[0] + (-100) * neighbors[1] + (0) * neighbors[2] + (100) * neighbors[3] +
                             (100) * neighbors[4] +
                             (-100) * neighbors[5] + (-100) * neighbors[6] + (0) * neighbors[7] + (100) * neighbors[8] +
                             (100) * neighbors[9] +
                             (-100) * neighbors[10] + (-100) * neighbors[11] + (0) * neighbors[12] +
                             (100) * neighbors[13] + (100) * neighbors[14] +
                             (-100) * neighbors[15] + (-100) * neighbors[16] + (0) * neighbors[17] +
                             (100) * neighbors[18] + (100) * neighbors[19] +
                             (-100) * neighbors[20] + (-100) * neighbors[21] + (0) * neighbors[22] +
                             (100) * neighbors[23] + (100) * neighbors[24];
                    var n4 = (-100) * neighbors[0] + (32) * neighbors[1] + (100) * neighbors[2] + (100) * neighbors[3] +
                             (100) * neighbors[4] +
                             (-100) * neighbors[5] + (-78) * neighbors[6] + (92) * neighbors[7] + (100) * neighbors[8] +
                             (100) * neighbors[9] +
                             (-100) * neighbors[10] + (-100) * neighbors[11] + (0) * neighbors[12] + (100) * neighbors[13] +
                             (100) * neighbors[14] +
                             (-100) * neighbors[15] + (-100) * neighbors[16] + (-92) * neighbors[17] + (78) * neighbors[18] +
                             (100) * neighbors[19] +
                             (-100) * neighbors[20] + (-100) * neighbors[21] + (-100) * neighbors[22] + (-32) * neighbors[23] +
                             (100) * neighbors[24];
                    var n5 = (100) * neighbors[0] + (100) * neighbors[1] + (100) * neighbors[2] + (100) * neighbors[3] +
                             (100) * neighbors[4] +
                             (-32) * neighbors[5] + (78) * neighbors[6] + (100) * neighbors[7] + (100) * neighbors[8] +
                             (100) * neighbors[9] +
                             (-100) * neighbors[10] + (-92) * neighbors[11] + (0) * neighbors[12] + (92) * neighbors[13] +
                             (100) * neighbors[14] +
                             (-100) * neighbors[15] + (-100) * neighbors[16] + (-100) * neighbors[17] + (-78) * neighbors[18] +
                             (32) * neighbors[19] +
                             (-100) * neighbors[20] + (-100) * neighbors[21] + (-100) * neighbors[22] + (-100) * neighbors[23] +
                             (-100) * neighbors[24];

                    nList.Add(n0);
                    nList.Add(n1);
                    nList.Add(n2);
                    nList.Add(n3);
                    nList.Add(n4);
                    nList.Add(n5);

                    // 取得 n0 ~ n5 的最大值並與 threshold 比較
                    var magnitude = nList.Max();
                    result.SetPixel(x, y, magnitude >= threshold ? Color.Black : Color.White);
                }
            }

            return result;
        }
    }
}
