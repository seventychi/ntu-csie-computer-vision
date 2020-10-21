using System;
using System.Drawing;
using System.IO;
using ScottPlot;

namespace Hw3
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                Directory.CreateDirectory("answers");

                using var image = new Bitmap("lena.bmp");

                // (A)
                image.Save(@"answers\a-1.bmp");
                GetHistogram(image)?.SaveFig(@"answers\a-2.bmp");

                // (B)
                var img = GetIntensityDivided3Image(image);
                img?.Save(@"answers\b-1.bmp");
                GetHistogram(img)?.SaveFig(@"answers\b-2.bmp");

                // (C)
                img = GetEqualizationImage(img);
                img?.Save(@"answers\c-1.bmp");
                GetHistogram(img)?.SaveFig(@"answers\c-2.bmp");


                Console.WriteLine("Histogram Equalization complete...");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadKey();
        }

        /// <summary>
        /// 取得直方圖
        /// </summary>
        /// <param name="srcImg">來源影像</param>
        /// <returns>Scott.Plot</returns>
        private static Plot GetHistogram(Bitmap srcImg)
        {
            if (srcImg == null)
                throw new Exception("source image is null");

            // xs: x 軸(0 ~ 255 的灰階值)
            var xs = new double[256];
            for (var x = 0; x < 256; x++) xs[x] = x;

            // ys: y 軸; 影像內灰階值對應的數量加總
            var ys = new double[256];

            for (var x = 0; x < srcImg.Width; x++)
            for (var y = 0; y < srcImg.Height; y++)
            {
                var color = srcImg.GetPixel(x, y);
                var gray = (color.R + color.G + color.B) / 3;
                ys[gray] += 1;
            }

            var plt = new Plot();
            plt.Title("Histogram");
            plt.XLabel("Gray (0~255)");
            plt.YLabel("Count");
            plt.PlotBar(xs, ys, barWidth: 1.4D, outlineWidth: 0);

            return plt;
        }

        /// <summary>
        /// 取得較暗的影像
        /// </summary>
        /// <param name="srcImg"></param>
        /// <returns></returns>
        private static Bitmap GetIntensityDivided3Image(Bitmap srcImg)
        {
            if (srcImg == null)
                throw new Exception("source image is null");

            var destImg = new Bitmap(srcImg);

            for (var x = 0; x < srcImg.Width; x++)
            for (var y = 0; y < srcImg.Height; y++)
            {
                var color = srcImg.GetPixel(x, y);
                destImg.SetPixel(x, y, Color.FromArgb(color.R / 3, color.G / 3, color.B / 3));
            }

            return destImg;
        }

        /// <summary>
        /// 取得 Histogram Equalization Image
        /// </summary>
        /// <param name="srcImg">來源影像</param>
        /// <returns>Histogram Equalization 影像</returns>
        private static Bitmap GetEqualizationImage(Bitmap srcImg)
        {
            // 機率分布
            var density = new double[256];
            var destImg = new Bitmap(srcImg);

            // 取得所有灰階值
            for (var x = 0; x < srcImg.Width; x++)
            for (var y = 0; y < srcImg.Height; y++)
            {
                var color = srcImg.GetPixel(x, y);
                var gray = (color.R + color.G + color.B) / 3;
                density[gray] += 1;
            }

            // 藉由灰階值數量計算機率
            for (var index = 0; index < 256; index++)
                density[index] = density[index] / (srcImg.Width * srcImg.Height * 1.0);

            for (var x = 0; x < srcImg.Width; x++)
            for (var y = 0; y < srcImg.Height; y++)
            {
                double sum = 0;
                var value = srcImg.GetPixel(x, y);
                for (var k = 0; k <= value.R; k++) sum += density[k];
                var rgb = (byte) Math.Round(255 * sum);
                destImg.SetPixel(x, y, Color.FromArgb(rgb, rgb, rgb));
            }

            return destImg;
        }
    }
}