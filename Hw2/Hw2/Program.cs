using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using ScottPlot;

namespace Hw2
{
    internal class Program
    {
        private static readonly Color WhiteColor = Color.FromArgb(255, 255, 255);
        private static readonly Color BlackColor = Color.FromArgb(0, 0, 0);
        private static int[,] _labels;

        private static void Main(string[] args)
        {
            try
            {
                Directory.CreateDirectory("answers");

                using var image = new Bitmap("lena.bmp");

                var binaryBitmap = GetBinaryBitmap(image);
                binaryBitmap?.Save(@"answers\a.bmp");
                GetHistogram(image)?.SaveFig(@"answers\b.bmp");
                GetConnectedComponents(binaryBitmap)?.SaveImage(@"answers\c.bmp");

                Console.WriteLine("image manipulation complete...");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadKey();
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
                dstBmp.SetPixel(x, y, gray >= 128 ? WhiteColor : BlackColor);
            }

            return dstBmp;
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
        /// 取得連通元件圖 (四連通)
        /// </summary>
        /// <param name="srcImg">來源影像</param>
        /// <returns>四連通圖(含 BoundingBox 及 Centroid Plus)</returns>
        private static Mat GetConnectedComponents(Bitmap srcImg)
        {
            // 初始化 int[,] 陣列，紀錄 (x, y) 的標籤值
            InitLabels(srcImg);

            var changed = true;

            // 判斷是否有值變更，若有則繼續遞迴更新標籤
            while (changed)
            {
                changed = UpDown();
                if (!changed) break;
                changed = BottomUp();
            }

            return GetBoundingBoxImage(srcImg);
        }

        /// <summary>
        /// 初始化 Labels
        /// </summary>
        /// <param name="srcImg"></param>
        private static void InitLabels(Bitmap srcImg)
        {
            _labels = new int[srcImg.Width, srcImg.Height];

            var label = 1;

            for (var y = 0; y < srcImg.Height; y++)
            for (var x = 0; x < srcImg.Width; x++)
            {
                if (srcImg.GetPixel(x, y) == BlackColor) continue;

                // 將 Pixel 值為 1 的標籤進行累加作為初始化
                _labels[x, y] = label;
                label += 1;
            }
        }

        /// <summary>
        /// 由上往下尋找四連通
        /// </summary>
        /// <returns></returns>
        private static bool UpDown()
        {
            var changed = false;

            for (var y = 0; y < _labels.GetLength(1); y++)
            for (var x = 0; x < _labels.GetLength(0); x++)
            {
                if (_labels[x, y] == 0) continue;

                var up = y - 1 >= 0 ? _labels[x, y - 1] : 0;
                var left = x - 1 >= 0 ? _labels[x - 1, y] : 0;
                var value = _labels[x, y];

                if (up != 0)
                {
                    if (left != 0)
                        value = up > left ? left : up;
                    else
                        value = up;
                }
                else
                {
                    if (left != 0) value = left;
                }

                if (_labels[x, y] == value) continue;

                _labels[x, y] = value;
                changed = true;
            }

            return changed;
        }

        /// <summary>
        /// 由下往上尋找四連通
        /// </summary>
        /// <returns></returns>
        private static bool BottomUp()
        {
            var changed = false;

            for (var y = _labels.GetLength(1) - 1; y >= 0; y--)
            for (var x = _labels.GetLength(0) - 1; x >= 0; x--)
            {
                if (_labels[x, y] == 0) continue;

                var bottom = y <= _labels.GetLength(1) - 2 ? _labels[x, y + 1] : 0;
                var right = x <= _labels.GetLength(0) - 2 ? _labels[x + 1, y] : 0;
                var value = _labels[x, y];

                if (bottom != 0)
                {
                    if (right != 0)
                        value = bottom > right ? right : bottom;
                    else
                        value = bottom;
                }
                else
                {
                    if (right != 0) value = right;
                }

                if (_labels[x, y] == value) continue;

                _labels[x, y] = value;
                changed = true;
            }

            return changed;
        }

        /// <summary>
        /// 繪製 BoundingBox
        /// </summary>
        /// <param name="srcImg"></param>
        private static Mat GetBoundingBoxImage(Bitmap srcImg)
        {
            var dic = new Dictionary<int, ConnectedComponent>();

            // 遞迴取出所有 ConnectedComponent
            for (var x = 0; x < _labels.GetLength(0); x++)
            for (var y = 0; y < _labels.GetLength(1); y++)
            {
                var value = _labels[x, y];
                var cmp = dic.ContainsKey(value) ? dic[value] : new ConnectedComponent();

                // 取得最左上值
                if (cmp.LeftTopPoint == null) cmp.LeftTopPoint = new Point(x, y);
                if (x < cmp.LeftTopPoint.X) cmp.LeftTopPoint.X = x;
                if (y < cmp.LeftTopPoint.Y) cmp.LeftTopPoint.Y = y;

                // 取得最右下值
                if (cmp.RightBottomPoint == null) cmp.RightBottomPoint = new Point(x, y);
                if (x > cmp.RightBottomPoint.X) cmp.RightBottomPoint.X = x;
                if (y > cmp.RightBottomPoint.Y) cmp.RightBottomPoint.Y = y;

                cmp.Points.Add(new Point(x, y));
                dic[value] = cmp;
            }

            // 篩選取得 >= 500 的 Connected Components
            var components = dic.Where(pair => pair.Key != 0 && pair.Value.Points.Count >= 500).Select(pair => pair.Value);
            var mat = srcImg.ToMat();

            foreach (var cmp in components)
            {
                // Bounding Box
                Cv2.Rectangle(
                    mat,
                    new OpenCvSharp.Point(cmp.LeftTopPoint.X, cmp.LeftTopPoint.Y),
                    new OpenCvSharp.Point(cmp.RightBottomPoint.X, cmp.RightBottomPoint.Y),
                    Scalar.Blue);

                // Centroid
                var x = 0;
                var y = 0;

                // 所有 x 及 y 值的加總後平均
                foreach (var point in cmp.Points)
                {
                    x += point.X;
                    y += point.Y;
                }

                x /= cmp.Points.Count;
                y /= cmp.Points.Count;

                Cv2.Circle(mat, new OpenCvSharp.Point(x, y), 5, Scalar.Red, 2);
            }

            return mat;
        }

        /// <summary>
        /// [DEBUG] 將四連通結果傳至 csv 輸出
        /// </summary>
        /// <param name="labels">標籤清單</param>
        private static void WriteConnectedComponentToCsv(int[,] labels)
        {
            var lines = new List<string>();

            for (var y = 0; y < 512; y++)
            {
                var line = "";

                for (var x = 0; x < 512; x++) line += $"{labels[x, y]},";

                lines.Add(line);
            }

            File.WriteAllLines("cmp.csv", lines);
        }
    }

    /// <summary>
    /// Connected Component
    /// </summary>
    public class ConnectedComponent
    {
        ///// <summary>
        ///// Component 內 Label 數量加總
        ///// </summary>
        //public int Count { get; set; }

        /// <summary>
        /// Connected Component 內含的點
        /// </summary>
        public List<Point> Points { get; set; } = new List<Point>();

        /// <summary>
        /// 最左上的 Point
        /// </summary>
        public Point LeftTopPoint { get; set; }

        /// <summary>
        /// 最右下的 Point
        /// </summary>
        public Point RightBottomPoint { get; set; }

        /// <summary>
        /// 重心 Point (所有 Point 的平均值)
        /// </summary>
        public Point CentroPoint { get; set; }
    }

    /// <summary>
    /// 座標
    /// </summary>
    public class Point
    {
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }
    }
}