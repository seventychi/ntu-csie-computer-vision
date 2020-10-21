using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Hw4
{
    internal class Program
    {
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

        private static readonly List<Point> L1 = new List<Point>
        {
            new Point(0, -1),
            new Point(0, 0),
            new Point(1, 0)
        };

        private static readonly List<Point> L2 = new List<Point>
        {
            new Point(1, -1),
            new Point(2, -1),
            new Point(1, -2)
        };

        private static void Main(string[] args)
        {
            try
            {
                Directory.CreateDirectory("answers");

                using var image = new Bitmap("lena.bmp");

                var binaryBitmap = GetBinaryBitmap(image);

                GetDilationBitmap(binaryBitmap, Octagon)?.Save(@"answers\a.bmp");
                GetErosionBitmap(binaryBitmap, Octagon)?.Save(@"answers\b.bmp");
                GetOpenBitmap(binaryBitmap, Octagon)?.Save(@"answers\c.bmp");
                GetClosingBitmap(binaryBitmap, Octagon)?.Save(@"answers\d.bmp");
                GetHitAndMissBitmap(binaryBitmap)?.Save(@"answers\e.bmp");

                Console.WriteLine("complete...");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 產生 Dilation 影像 (膨脹影像)
        /// </summary>
        /// <param name="srcImg">來源圖片</param>
        /// <param name="kernel">kernel</param>
        /// <returns>Dilation 影像</returns>
        private static Bitmap GetDilationBitmap(Bitmap srcImg, IList<Point> kernel)
        {
            var result = new Bitmap(srcImg);

            for (var x = 0; x < srcImg.Width; x++)
            for (var y = 0; y < srcImg.Height; y++)
            {
                // 判斷是否為白色
                if (srcImg.GetPixel(x, y).R != 255) continue;

                foreach (var point in kernel)
                {
                    var px = x + point.X;
                    var py = y + point.Y;

                    if (px >= 0 && px < srcImg.Width && py >= 0 && py < srcImg.Height)
                        result.SetPixel(px, py, Color.White);
                }
            }

            return result;
        }

        /// <summary>
        /// 產生 Erosion 影像 (侵蝕影像)
        /// </summary>
        /// <param name="srcImg">來源影像</param>
        /// <param name="kernel">kernel</param>
        /// <returns>Erosion 影像</returns>
        private static Bitmap GetErosionBitmap(Bitmap srcImg, IList<Point> kernel)
        {
            var result = CreateBlankBitmap(srcImg.Width, srcImg.Height);

            for (var x = 0; x < srcImg.Width; x++)
            for (var y = 0; y < srcImg.Height; y++)
            {
                var contained = true;

                foreach (var point in kernel)
                {
                    var px = x + point.X;
                    var py = y + point.Y;

                    if (px < 0 ||
                        px >= srcImg.Width ||
                        py < 0 ||
                        py >= srcImg.Height ||
                        srcImg.GetPixel(px, py).R != 255)
                    {
                        contained = false;
                        break;
                    }
                }

                if (contained)
                    result.SetPixel(x, y, Color.White);
            }

            return result;
        }

        /// <summary>
        /// 取得 Open Bitmap (先 Erosion 再 Dilation)
        /// </summary>
        /// <param name="srcImg">來源影像</param>
        /// <param name="kernel">結構</param>
        /// <returns></returns>
        private static Bitmap GetOpenBitmap(Bitmap srcImg, IList<Point> kernel)
        {
            return GetDilationBitmap(GetErosionBitmap(srcImg, kernel), kernel);
        }

        /// <summary>
        /// 取得 Closing Bitmap (先 Dilation 再 Erosion)
        /// </summary>
        /// <param name="srcImg">來源影像</param>
        /// <param name="kernel">結構</param>
        /// <returns></returns>
        private static Bitmap GetClosingBitmap(Bitmap srcImg, IList<Point> kernel)
        {
            return GetErosionBitmap(GetDilationBitmap(srcImg, kernel), kernel);
        }

        /// <summary>
        /// 取得 HitAndMiss Bitmap
        /// </summary>
        /// <param name="srcImg"></param>
        /// <returns></returns>
        private static Bitmap GetHitAndMissBitmap(Bitmap srcImg)
        {
            var result = CreateBlankBitmap(srcImg.Width, srcImg.Height);
            var cmpImg = new Bitmap(srcImg);

            for (var x = 0; x < srcImg.Width; x++)
            for (var y = 0; y < srcImg.Height; y++)
                cmpImg.SetPixel(x, y, srcImg.GetPixel(x, y).R == 255 ? Color.Black : Color.White);

            var e1 = GetErosionBitmap(srcImg, L1);
            var e2 = GetErosionBitmap(cmpImg, L2);

            for (var x = 0; x < srcImg.Width; x++)
            for (var y = 0; y < srcImg.Height; y++)
                if (e1.GetPixel(x, y).R == 255 && e2.GetPixel(x, y).R == 255)
                    result.SetPixel(x, y, Color.White);

            return result;
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
        /// 建立黑底的圖片
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private static Bitmap CreateBlankBitmap(int width, int height)
        {
            var img = new Bitmap(width, height);

            for (var x = 0; x < width; x++)
            for (var y = 0; y < height; y++)
                img.SetPixel(x, y, Color.Black);

            return img;
        }
    }
}