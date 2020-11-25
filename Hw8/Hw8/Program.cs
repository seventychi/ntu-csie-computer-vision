using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using MathNet.Numerics.Distributions;

namespace Hw8
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

        private static void Main(string[] args)
        {
            try
            {
                Directory.CreateDirectory("answers");

                var image = new Bitmap("lena.bmp");

                var gaussian10Img = GetGaussianNoiseImage(image, 10);
                var gaussian30Img = GetGaussianNoiseImage(image, 30);
                var sap01Img = GetSaltAndPepperImage(image, 0.1);
                var sap005Img = GetSaltAndPepperImage(image, 0.05);
                var bf33Gaussian10Img = GetBoxFilterImage(gaussian10Img, 3, 3);
                var bf55Gaussian10Img = GetBoxFilterImage(gaussian10Img, 5, 5);
                var bf33Gaussian30Img = GetBoxFilterImage(gaussian30Img, 3, 3);
                var bf55Gaussian30Img = GetBoxFilterImage(gaussian30Img, 5, 5);
                var bf33Sap01Img = GetBoxFilterImage(sap01Img, 3, 3);
                var bf55Sap01Img = GetBoxFilterImage(sap01Img, 5, 5);
                var bf33Sap005Img = GetBoxFilterImage(sap005Img, 3, 3);
                var bf55Sap005Img = GetBoxFilterImage(sap005Img, 5, 5);
                var md33Gaussian10Img = GetMeidianImage(gaussian10Img, 3, 3);
                var md55Gaussian10Img = GetMeidianImage(gaussian10Img, 5, 5);
                var md33Gaussian30Img = GetMeidianImage(gaussian30Img, 3, 3);
                var md55Gaussian30Img = GetMeidianImage(gaussian30Img, 5, 5);
                var md33Sap01Img = GetMeidianImage(sap01Img, 3, 3);
                var md55Sap01Img = GetMeidianImage(sap01Img, 5, 5);
                var md33Sap005Img = GetMeidianImage(sap005Img, 3, 3);
                var md55Sap005Img = GetMeidianImage(sap005Img, 5, 5);
                var ocGaussian10Img = GetOpeningThenClosingImage(gaussian10Img, Octagon);
                var ocGaussian30Img = GetOpeningThenClosingImage(gaussian30Img, Octagon);
                var ocSap01Img = GetOpeningThenClosingImage(sap01Img, Octagon);
                var ocSap005Img = GetOpeningThenClosingImage(sap005Img, Octagon);
                var coGaussian10Img = GetClosingThenOpeningImage(gaussian10Img, Octagon);
                var coGaussian30Img = GetClosingThenOpeningImage(gaussian30Img, Octagon);
                var coSap01Img = GetClosingThenOpeningImage(sap01Img, Octagon);
                var coSap005Img = GetClosingThenOpeningImage(sap005Img, Octagon);

                gaussian10Img.Save(@"answers\a_gaussian_10.png");
                gaussian30Img.Save(@"answers\a_gaussian_30.png");
                sap01Img.Save(@"answers\b_saltandpepper_0.1.png");
                sap005Img.Save(@"answers\b_saltandpepper_0.05.png");
                bf33Gaussian10Img.Save(@"answers\c_boxfilter_33_gaussian_10.png");
                bf55Gaussian10Img.Save(@"answers\c_boxfilter_55_gaussian_10.png");
                bf33Gaussian30Img.Save(@"answers\c_boxfilter_33_gaussian_30.png");
                bf55Gaussian30Img.Save(@"answers\c_boxfilter_55_gaussian_30.png");
                bf33Sap01Img.Save(@"answers\c_boxfilter_33_saltandpepper_0.1.png");
                bf55Sap01Img.Save(@"answers\c_boxfilter_55_saltandpepper_0.1.png");
                bf33Sap005Img.Save(@"answers\c_boxfilter_33_saltandpepper_0.05.png");
                bf55Sap005Img.Save(@"answers\c_boxfilter_55_saltandpepper_0.05.png");
                md33Gaussian10Img.Save(@"answers\d_medianfilter_33_gaussian_10.png");
                md55Gaussian10Img.Save(@"answers\d_medianfilter_55_gaussian_10.png");
                md33Gaussian30Img.Save(@"answers\d_medianfilter_33_gaussian_30.png");
                md55Gaussian30Img.Save(@"answers\d_medianfilter_55_gaussian_30.png");
                md33Sap01Img.Save(@"answers\d_medianfilter_33_saltandpepper_0.1.png");
                md55Sap01Img.Save(@"answers\d_medianfilter_55_saltandpepper_0.1.png");
                md33Sap005Img.Save(@"answers\d_medianfilter_33_saltandpepper_0.05.png");
                md55Sap005Img.Save(@"answers\d_medianfilter_55_saltandpepper_0.05.png");
                ocGaussian10Img.Save(@"answers\e_openingThenClosing_gaussian_10.png");
                ocGaussian30Img.Save(@"answers\e_openingThenClosing_gaussian_30.png");
                ocSap01Img.Save(@"answers\e_openingThenClosing_saltandpepper_0.1.png");
                ocSap005Img.Save(@"answers\e_openingThenClosing_saltandpepper_0.05.png");
                coGaussian10Img.Save(@"answers\e_closingThenOpening_gaussian_10.png");
                coGaussian30Img.Save(@"answers\e_closingThenOpening_gaussian_30.png");
                coSap01Img.Save(@"answers\e_closingThenOpening_saltandpepper_0.1.png");
                coSap005Img.Save(@"answers\e_closingThenOpening_saltandpepper_0.05.png");

                var gaussian10ImgSnr = GetSNR(image, gaussian10Img);
                var gaussian30ImgSnr = GetSNR(image, gaussian30Img);
                var sap01ImgSnr = GetSNR(image, sap01Img);
                var sap005ImgSnr = GetSNR(image, sap005Img);
                var bf33Gaussian10ImgSnr = GetSNR(image, bf33Gaussian10Img);
                var bf55Gaussian10ImgSnr = GetSNR(image, bf55Gaussian10Img);
                var bf33Gaussian30ImgSnr = GetSNR(image, bf33Gaussian30Img);
                var bf55Gaussian30ImgSnr = GetSNR(image, bf55Gaussian30Img);
                var bf33Sap01ImgSnr = GetSNR(image, bf33Sap01Img);
                var bf55Sap01ImgSnr = GetSNR(image, bf55Sap01Img);
                var bf33Sap005ImgSnr = GetSNR(image, bf33Sap005Img);
                var bf55Sap005ImgSnr = GetSNR(image, bf55Sap005Img);
                var md33Gaussian10ImgSnr = GetSNR(image, md33Gaussian10Img);
                var md55Gaussian10ImgSnr = GetSNR(image, md55Gaussian10Img);
                var md33Gaussian30ImgSnr = GetSNR(image, md33Gaussian30Img);
                var md55Gaussian30ImgSnr = GetSNR(image, md55Gaussian30Img);
                var md33Sap01ImgSnr = GetSNR(image, md33Sap01Img);
                var md55Sap01ImgSnr = GetSNR(image, md55Sap01Img);
                var md33Sap005ImgSnr = GetSNR(image, md33Sap005Img);
                var md55Sap005ImgSnr = GetSNR(image, md55Sap005Img);
                var ocGaussian10ImgSnr = GetSNR(image, ocGaussian10Img);
                var ocGaussian30ImgSnr = GetSNR(image, ocGaussian30Img);
                var ocSap01ImgSnr = GetSNR(image, ocSap01Img);
                var ocSap005ImgSnr = GetSNR(image, ocSap005Img);
                var coGaussian10ImgSnr = GetSNR(image, coGaussian10Img);
                var coGaussian30ImgSnr = GetSNR(image, coGaussian30Img);
                var coSap01ImgSnr = GetSNR(image, coSap01Img);
                var coSap005ImgSnr = GetSNR(image, coSap005Img);

                var lines = new List<string>
                {
                    $"gaussian10ImgSnr,{gaussian10ImgSnr}",
                    $"gaussian30ImgSnr,{gaussian30ImgSnr}",
                    $"sap01ImgSnr,{sap01ImgSnr}",
                    $"sap005ImgSnr,{sap005ImgSnr}",
                    $"bf33Gaussian10ImgSnr,{bf33Gaussian10ImgSnr}",
                    $"bf55Gaussian10ImgSnr,{bf55Gaussian10ImgSnr}",
                    $"bf33Gaussian30ImgSnr,{bf33Gaussian30ImgSnr}",
                    $"bf55Gaussian30ImgSnr,{bf55Gaussian30ImgSnr}",
                    $"bf33Sap01ImgSnr,{bf33Sap01ImgSnr}",
                    $"bf55Sap01ImgSnr,{bf55Sap01ImgSnr}",
                    $"bf33Sap005ImgSnr,{bf33Sap005ImgSnr}",
                    $"bf55Sap005ImgSnr,{bf55Sap005ImgSnr}",
                    $"md33Gaussian10ImgSnr,{md33Gaussian10ImgSnr}",
                    $"md55Gaussian10ImgSnr,{md55Gaussian10ImgSnr}",
                    $"md33Gaussian30ImgSnr,{md33Gaussian30ImgSnr}",
                    $"md55Gaussian30ImgSnr,{md55Gaussian30ImgSnr}",
                    $"md33Sap01ImgSnr,{md33Sap01ImgSnr}",
                    $"md55Sap01ImgSnr,{md55Sap01ImgSnr}",
                    $"md33Sap005ImgSnr,{md33Sap005ImgSnr}",
                    $"md55Sap005ImgSnr,{md55Sap005ImgSnr}",
                    $"ocGaussian10ImgSnr,{ocGaussian10ImgSnr}",
                    $"ocGaussian30ImgSnr,{ocGaussian30ImgSnr}",
                    $"ocSap01ImgSnr,{ocSap01ImgSnr}",
                    $"ocSap005ImgSnr,{ocSap005ImgSnr}",
                    $"coGaussian10ImgSnr,{coGaussian10ImgSnr}",
                    $"coGaussian30ImgSnr,{coGaussian30ImgSnr}",
                    $"coSap01ImgSnr,{coSap01ImgSnr}",
                    $"coSap005ImgSnr,{coSap005ImgSnr}"
                };

                File.WriteAllLines(@"answers\snr.csv", lines);

                Console.WriteLine("completed...");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadKey();
        }

        /// <summary>
        /// 取得高斯雜訊影像
        /// </summary>
        /// <param name="srcImg"></param>
        /// <param name="amplitude"></param>
        /// <returns></returns>
        private static Bitmap GetGaussianNoiseImage(Bitmap srcImg, double amplitude)
        {
            var result = new Bitmap(srcImg);

            for (var x = 0; x < srcImg.Width; x++)
                for (var y = 0; y < srcImg.Height; y++)
                {
                    var noise = Convert.ToInt32(srcImg.GetPixel(x, y).R + amplitude * new Normal(0, 1).Sample());
                    noise = noise > 255 ? 255 : noise;
                    noise = noise <= 0 ? 0 : noise;

                    result.SetPixel(x, y, Color.FromArgb(noise, noise, noise));
                }

            return result;
        }

        /// <summary>
        /// 取得白鹽黑胡椒影像
        /// </summary>
        /// <param name="srcImg"></param>
        /// <param name="amplitude"></param>
        /// <returns></returns>
        private static Bitmap GetSaltAndPepperImage(Bitmap srcImg, double probability)
        {
            var result = new Bitmap(srcImg);
            var random = new Random();

            for (var x = 0; x < srcImg.Width; x++)
                for (var y = 0; y < srcImg.Height; y++)
                {
                    var value = random.NextDouble();
                    if (value <= probability)
                        result.SetPixel(x, y, Color.Black);
                    else if (value >= 1 - probability) result.SetPixel(x, y, Color.White);
                }

            return result;
        }

        /// <summary>
        /// 取得盒子濾波影像
        /// </summary>
        /// <param name="srcImg"></param>
        /// <param name="amplitude"></param>
        /// <returns></returns>
        private static Bitmap GetBoxFilterImage(Bitmap srcImg, int boxWidth, int boxHeight)
        {
            var result = new Bitmap(srcImg);

            var centerX = (boxWidth - 1) / 2;
            var centerY = (boxHeight - 1) / 2;

            for (var x = 0; x < srcImg.Width; x++)
                for (var y = 0; y < srcImg.Height; y++)
                {
                    var boxPixels = new List<int>();

                    for (var bx = 0; bx < boxWidth; bx++)
                        for (var by = 0; @by < boxHeight; @by++)
                        {
                            var destX = x + bx - centerX;
                            var destY = y + @by - centerY;

                            if (destX < 0 || destX >= srcImg.Width || destY < 0 || destY >= srcImg.Height) continue;

                            boxPixels.Add(srcImg.GetPixel(destX, destY).R);
                        }

                    var value = boxPixels.Count == 0 ? 0 : boxPixels.Sum() / boxPixels.Count;
                    result.SetPixel(x, y, Color.FromArgb(value, value, value));
                }

            return result;
        }

        /// <summary>
        /// 取得中值濾波影像
        /// </summary>
        /// <param name="srcImg"></param>
        /// <param name="boxWidth"></param>
        /// <param name="boxHeight"></param>
        /// <returns></returns>
        private static Bitmap GetMeidianImage(Bitmap srcImg, int boxWidth, int boxHeight)
        {
            var result = new Bitmap(srcImg);

            var centerX = (boxWidth - 1) / 2;
            var centerY = (boxHeight - 1) / 2;

            for (var x = 0; x < srcImg.Width; x++)
                for (var y = 0; y < srcImg.Height; y++)
                {
                    var boxPixels = new List<int>();

                    for (var bx = 0; bx < boxWidth; bx++)
                        for (var by = 0; @by < boxHeight; @by++)
                        {
                            var destX = x + bx - centerX;
                            var destY = y + @by - centerY;

                            if (destX < 0 || destX >= srcImg.Width || destY < 0 || destY >= srcImg.Height) continue;

                            boxPixels.Add(srcImg.GetPixel(destX, destY).R);
                        }

                    boxPixels.Sort();

                    var value = boxPixels.Count == 0 ? 0 : boxPixels[boxPixels.Count / 2];
                    result.SetPixel(x, y, Color.FromArgb(value, value, value));
                }

            return result;
        }

        /// <summary>
        /// 取得先 Opening 再 Closing 的影像
        /// </summary>
        /// <param name="srcImg"></param>
        /// <param name="kernel"></param>
        /// <returns></returns>
        private static Bitmap GetOpeningThenClosingImage(Bitmap srcImg, IList<Point> kernel)
        {
            return GetGrayClosingBitmap(GetGrayOpeningBitmap(srcImg, kernel), kernel);
        }

        /// <summary>
        /// 取得先 Closing 再 Opening 的影像
        /// </summary>
        /// <param name="srcImg"></param>
        /// <param name="kernel"></param>
        /// <returns></returns>
        private static Bitmap GetClosingThenOpeningImage(Bitmap srcImg, IList<Point> kernel)
        {
            return GetGrayOpeningBitmap(GetGrayClosingBitmap(srcImg, kernel), kernel);
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

                            if (max < tmp) max = tmp;
                        }
                    }

                    result.SetPixel(x, y, Color.FromArgb(max, max, max));
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
                            if (tmp < min) min = tmp;
                        }
                    }

                    if (min < 0) min = 0;

                    result.SetPixel(x, y, Color.FromArgb(min, min, min));
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

        /// <summary>
        /// 取得 SNR
        /// </summary>
        /// <param name="srcImg"></param>
        /// <param name="noiseImg"></param>
        /// <returns></returns>
        private static double GetSNR(Bitmap srcImg, Bitmap noiseImg)
        {
            double muSignal = 0;
            double powerSignal = 0;
            double muNoise = 0;
            double powerNoise = 0;

            for (var x = 0; x < srcImg.Width; x++)
                for (var y = 0; y < srcImg.Height; y++)
                    muSignal += srcImg.GetPixel(x, y).R;

            muSignal /= (srcImg.Width * srcImg.Height);

            for (var x = 0; x < noiseImg.Width; x++)
                for (var y = 0; y < noiseImg.Height; y++)
                    muNoise += (noiseImg.GetPixel(x, y).R - srcImg.GetPixel(x, y).R);

            muNoise /= noiseImg.Width * noiseImg.Height;

            for (var x = 0; x < srcImg.Width; x++)
                for (var y = 0; y < srcImg.Height; y++)
                    powerSignal += Math.Pow(srcImg.GetPixel(x, y).R - muSignal, 2);

            powerSignal /= srcImg.Width * srcImg.Height;

            for (var x = 0; x < noiseImg.Width; x++)
                for (var y = 0; y < noiseImg.Height; y++)
                    powerNoise += Math.Pow(noiseImg.GetPixel(x, y).R - srcImg.GetPixel(x, y).R - muNoise, 2);

            powerNoise /= noiseImg.Width * noiseImg.Height;

            return 20 * Math.Log(Math.Sqrt(powerSignal) / Math.Sqrt(powerNoise), 10);
        }

        private class Point
        {
            public Point(int x, int y, int value = 0)
            {
                X = x;
                Y = y;
                Value = value;
            }

            public int X { get; }

            public int Y { get; }

            public int Value { get; }
        }
    }
}