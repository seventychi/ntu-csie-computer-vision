using System;
using System.Drawing;
using System.IO;

namespace Hw7
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                Directory.CreateDirectory("answers");

                var image = GetDownSamplingBitmap(GetBinaryBitmap(new Bitmap("lena.bmp")));

                var iteration = 1;

                while (true)
                {
                    // 產生 Yokoi Matrix
                    var yokoi = GetYokoiMatrix(image);

                    // 透過 Yokoi 產生 Pair Relation Matrix
                    var prm = GetPairRelationMatrix(yokoi);

                    // 將原始影像與 Pair Relation 的 P 比對後做 Shrinking
                    var shrink = GetShrinkImage(image, prm);

                    // 比對 Shrink 的結果是否與最後一次的影像相同, 相同則停止進行 Shrinking
                    if (Diff(image, shrink))
                    {
                        break;
                    }

                    image = shrink;
                    image.Save($@"answers\{iteration}.png");

                    Console.WriteLine($"iteration: {iteration}");

                    iteration += 1;
                }

                image?.Dispose();

                Console.WriteLine("completed...");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadKey();
        }

        #region Step 1. YOKOI

        /// <summary>
        /// 取得 Yokoi Connectivity Number Matrix
        /// </summary>
        private static int[,] GetYokoiMatrix(Bitmap srcImg)
        {
            var result = new int[srcImg.Width, srcImg.Height];

            for (var x = 0; x < srcImg.Width; x++)
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

                    var a1 = YokoiH(b, c, d, e);

                    // a2
                    c = e;
                    d = x >= 1 && y >= 1 ? srcImg.GetPixel(x - 1, y - 1).R : 0;
                    e = x >= 1 ? srcImg.GetPixel(x - 1, y).R : 0;

                    var a2 = YokoiH(b, c, d, e);

                    // a3
                    c = e;
                    d = x >= 1 && y < srcImg.Height - 1 ? srcImg.GetPixel(x - 1, y + 1).R : 0;
                    e = y < srcImg.Height - 1 ? srcImg.GetPixel(x, y + 1).R : 0;

                    var a3 = YokoiH(b, c, d, e);

                    // a4
                    c = e;
                    d = x < srcImg.Width - 1 && y < srcImg.Height - 1 ? srcImg.GetPixel(x + 1, y + 1).R : 0;
                    e = x < srcImg.Width - 1 ? srcImg.GetPixel(x + 1, y).R : 0;

                    var a4 = YokoiH(b, c, d, e);
                    result[x, y] = YokoiF(a1, a2, a3, a4);
                }

            return result;
        }

        /// <summary>
        /// 根據 h(b, c, d, e) 的算式求得 q, r, s
        /// </summary>
        /// <param name="b">x0, x1, x6, x2</param>
        /// <param name="c">x0, x2, x7, x3</param>
        /// <param name="d">x0, x3, x8, x4</param>
        /// <param name="e">x0, x4, x5, x1</param>
        /// <returns>取得 Yokoi H Function 的值</returns>
        private static char YokoiH(int b, int c, int d, int e)
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
        /// <param name="a1">h(x0, x1, x6, x2)</param>
        /// <param name="a2">h(x0, x2, x7, x3)</param>
        /// <param name="a3">h(x0, x3, x8, x4)</param>
        /// <param name="a4">h(x0, x4, x5, x1)</param>
        /// <returns></returns>
        private static int YokoiF(char a1, char a2, char a3, char a4)
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

        #endregion

        #region Step 2. Pair Relation Matrix

        /// <summary>
        /// 取得 Pair Relation Matrix
        /// </summary>
        /// <param name="yokoi">YOKOI matrix</param>
        /// <returns></returns>
        private static char[,] GetPairRelationMatrix(int[,] yokoi)
        {
            var width = yokoi.GetLength(0);
            var height = yokoi.GetLength(1);
            var prMatrix = new char[width, height];

            for (var x = 0; x < width; x++)
            for (var y = 0; y < height; y++)
            {
                if (yokoi[x, y] == 0)
                    continue;

                var x0 = yokoi[x, y];
                var x1 = x + 1 < width ? yokoi[x + 1, y] : 0;
                var x2 = y - 1 >= 0 ? yokoi[x, y - 1] : 0;
                var x3 = x - 1 >= 0 ? yokoi[x - 1, y] : 0;
                var x4 = y + 1 < height ? yokoi[x, y + 1] : 0;

                var h1 = x1 == 1 ? 1 : 0;
                var h2 = x2 == 1 ? 1 : 0;
                var h3 = x3 == 1 ? 1 : 0;
                var h4 = x4 == 1 ? 1 : 0;

                prMatrix[x, y] = h1 + h2 + h3 + h4 < 1 || x0 != 1 ? 'q' : 'p';
            }

            return prMatrix;
        }

        #endregion

        #region Step 3. Connected Shrink Operator

        private static Bitmap GetShrinkImage(Bitmap srcImg, char[,] markedImg)
        {
            var result = new Bitmap(srcImg);

            for (var y = 0; y < result.Height; y++)
            {
                for (var x = 0; x < result.Width; x++)
                {
                    if (markedImg[x, y] == 'p')
                    {
                        var x0 = result.GetPixel(x, y).R;

                        // a1
                        var b = x0;
                        var c = x < result.Width - 1 ? result.GetPixel(x + 1, y).R : 0;
                        var d = x < result.Width - 1 && y >= 1 ? result.GetPixel(x + 1, y - 1).R : 0;
                        var e = y >= 1 ? result.GetPixel(x, y - 1).R : 0;

                        var a1 = ShrinkH(b, c, d, e);

                        // a2
                        c = e;
                        d = x >= 1 && y >= 1 ? result.GetPixel(x - 1, y - 1).R : 0;
                        e = x >= 1 ? result.GetPixel(x - 1, y).R : 0;

                        var a2 = ShrinkH(b, c, d, e);

                        // a3
                        c = e;
                        d = x >= 1 && y < result.Height - 1 ? result.GetPixel(x - 1, y + 1).R : 0;
                        e = y < result.Height - 1 ? result.GetPixel(x, y + 1).R : 0;

                        var a3 = ShrinkH(b, c, d, e);

                        // a4
                        c = e;
                        d = x < result.Width - 1 && y < result.Height - 1 ? result.GetPixel(x + 1, y + 1).R : 0;
                        e = x < result.Width - 1 ? result.GetPixel(x + 1, y).R : 0;

                        var a4 = ShrinkH(b, c, d, e);


                        result.SetPixel(x, y, ShrinkF(a1, a2, a3, a4));
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Shrink H Function
        /// </summary>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private static int ShrinkH(int b, int c, int d, int e)
        {
            return b == c && (d != b || e != b) ? 1 : 0;
        }

        /// <summary>
        /// Shrink F Function
        /// </summary>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <param name="a3"></param>
        /// <param name="a4"></param>
        /// <returns></returns>
        private static Color ShrinkF(int a1, int a2, int a3, int a4)
        {
            var sum = a1 + a2 + a3 + a4;
            return sum == 1 ? Color.Black : Color.White;
        }

        #endregion

        /// <summary>
        /// 確認兩張圖是否相同
        /// </summary>
        /// <param name="img1"></param>
        /// <param name="img2"></param>
        /// <returns></returns>
        private static bool Diff(Bitmap img1, Bitmap img2)
        {
            for (var x = 0; x < img1.Width; x++)
            {
                for (var y = 0; y < img1.Height; y++)
                {
                    if (img1.GetPixel(x, y).R != img2.GetPixel(x, y).R)
                        return false;
                }
            }

            return true;
        }

        #region image execution

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
            for (var y = 0; y < srcImg.Height; y += 8)
                dstImg.SetPixel(x / 8, y / 8, srcImg.GetPixel(x, y));

            return dstImg;
        }

        /// <summary>
        /// 取得範例圖片做測試
        /// </summary>
        /// <returns></returns>
        private static Bitmap GetSampleImage()
        {
            var image = new Bitmap(5, 5);
            for (var x = 0; x < 5; x++)
            for (var y = 0; y < 5; y++)
                image.SetPixel(x, y, Color.Black);

            image.SetPixel(0, 0, Color.White);
            image.SetPixel(1, 0, Color.White);
            image.SetPixel(1, 1, Color.White);
            image.SetPixel(2, 1, Color.White);
            image.SetPixel(2, 2, Color.White);
            image.SetPixel(3, 2, Color.White);
            image.SetPixel(2, 3, Color.White);
            image.SetPixel(3, 3, Color.White);
            image.SetPixel(2, 4, Color.White);
            image.SetPixel(3, 4, Color.White);
            return image;
        }

        #endregion
    }
}