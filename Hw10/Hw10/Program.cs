using System;
using System.Drawing;
using System.IO;

namespace Hw10
{
    internal class Program
    {
        private static readonly int[][] LaplacianMask1Kernel = {
            new[] {0, 1, 0},
            new[] {1, -4, 1},
            new[] {0, 1, 0}
        };

        private static readonly int[][] LaplacianMask2Kernel = {
            new[] {1, 1, 1},
            new[] {1, -8, 1},
            new[] {1, 1, 1}
        };

        private static readonly int[][] MinimumVarianceLaplacianKernel =
        {
            new[] {2, -1, 2},
            new[] {-1, -4, -1},
            new[] {2, -1, 2}
        };

        private static readonly int[][] LaplacianOfGaussiankernel = {
            new[] {0, 0, 0, -1, -1, -2, -1, -1, 0, 0, 0},
            new[] {0, 0, -2, -4, -8, -9, -8, -4, -2, 0, 0},
            new[] {0, -2, -7, -15, -22, -23, -22, -15, -7, -2, 0},
            new[] {-1, -4, -15, -24, -14, -1, -14, -24, -15, -4, -1},
            new[] {-1, -8, -22, -14, 52, 103, 52, -14, -22, -8, -1},
            new[] {-2, -9, -23, -1, 103, 178, 103, -1, -23, -9, -2},
            new[] {-1, -8, -22, -14, 52, 103, 52, -14, -22, -8, -1},
            new[] {-1, -4, -15, -24, -14, -1, -14, -24, -15, -4, -1},
            new[] {0, -2, -7, -15, -22, -23, -22, -15, -7, -2, 0},
            new[] {0, 0, -2, -4, -8, -9, -8, -4, -2, 0, 0},
            new[] {0, 0, 0, -1, -1, -2, -1, -1, 0, 0, 0}
        };

        private static readonly int[][] DifferenceOfGaussianKernel =
        {
            new[] {-1, -3, -4, -6, -7, -8, -7, -6, -4, -3, -1},
            new[] {-3, -5, -8, -11, -13, -13, -13, -11, -8, -5, -3},
            new[] {-4, -8, -12, -16, -17, -17, -17, -16, -12, -8, -4},
            new[] {-6, -11, -16, -16, 0, 15, 0, -16, -16, -11, -6},
            new[] {-7, -13, -17, 0, 85, 160, 85, 0, -17, -13, -7},
            new[] {-8, -13, -17, 15, 160, 283, 160, 15, -17, -13, -8},
            new[] {-7, -13, -17, 0, 85, 160, 85, 0, -17, -13, -7},
            new[] {-6, -11, -16, -16, 0, 15, 0, -16, -16, -11, -6},
            new[] {-4, -8, -12, -16, -17, -17, -17, -16, -12, -8, -4},
            new[] {-3, -5, -8, -11, -13, -13, -13, -11, -8, -5, -3},
            new[] {-1, -3, -4, -6, -7, -8, -7, -6, -4, -3, -1}
        };

        private static void Main(string[] args)
        {
            try
            {
                Directory.CreateDirectory("answers");

                using var image = new Bitmap("lena.bmp");

                GetZeroCrossingDetectorImage(GetLaplacianArray(image, 15, LaplacianMask1Kernel))?.Save(@"answers\a.png");
                GetZeroCrossingDetectorImage(GetLaplacianArray(image, 15, LaplacianMask2Kernel, 3))?.Save(@"answers\b.png");
                GetZeroCrossingDetectorImage(GetLaplacianArray(image, 20, MinimumVarianceLaplacianKernel, 3))?.Save(@"answers\c.png");
                GetZeroCrossingDetectorImage(GetLaplacianArray(image, 3000, LaplacianOfGaussiankernel))?.Save(@"answers\d.png");
                GetZeroCrossingDetectorImage(GetLaplacianArray(image, 1, DifferenceOfGaussianKernel))?.Save(@"answers\e.png");

                Console.WriteLine("complete...");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 取得 Laplacian Array
        /// </summary>
        /// <param name="srcImg"></param>
        /// <param name="threshold"></param>
        /// <param name="kernel"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        private static int[,] GetLaplacianArray(Bitmap srcImg, int threshold, int[][] kernel, int weight = 1)
        {
            var kWidth = kernel.GetLength(0);
            var kHeight = kernel[0].GetLength(0);

            var result = new int[srcImg.Width, srcImg.Height];

            for (var x = 0; x < srcImg.Width; x++)
            {
                for (var y = 0; y < srcImg.Height; y++)
                {
                    #region get xn, yn

                    var xn = new int[kWidth];
                    var yn = new int[kHeight];

                    var distance = -kWidth / 2;
                    
                    for (var i = 0; i < xn.Length; i++)
                    {
                        if (distance <= 0)
                        {
                            xn[i] = Math.Max(x + distance, 0);
                        }
                        else
                        {
                            xn[i] = Math.Min(x + distance, srcImg.Width - 1);
                        }

                        distance += 1;
                    }

                    distance = -kHeight / 2;

                    for (var i = 0; i < yn.Length; i++)
                    {
                        if (distance <= 0)
                        {
                            yn[i] = Math.Max(y + distance, 0);
                        }
                        else
                        {
                            yn[i] = Math.Min(y + distance, srcImg.Height - 1);
                        }

                        distance += 1;
                    }

                    #endregion

                    #region get neighbors

                    var neighbors = new int[kWidth, kHeight];

                    for (var i = 0; i < kWidth; i++)
                    {
                        for (var j = 0; j < kHeight; j++)
                        {
                            neighbors[i, j] = srcImg.GetPixel(xn[i], yn[j]).R;
                        }
                    }

                    #endregion

                    var magnitude = 0;

                    for (var i = 0; i < kWidth; i++)
                    {
                        for (var j = 0; j < kHeight; j++)
                        {
                            magnitude += kernel[j][i] * neighbors[i, j];
                        }
                    }

                    magnitude /= weight;

                    if (magnitude >= threshold)
                        result[x, y] = 1;
                    else if (magnitude <= -threshold)
                        result[x, y] = -1;
                    else
                        result[x, y] = 0;
                }
            }

            return result;
        }

        /// <summary>
        /// 取得 Zero Crossing Detector Image
        /// </summary>
        /// <param name="laplacianArray"></param>
        /// <returns></returns>
        private static Bitmap GetZeroCrossingDetectorImage(int[,] laplacianArray)
        {
            var result = new Bitmap(laplacianArray.GetLength(0), laplacianArray.GetLength(1));

            for (var x = 0; x < result.Width; x++)
            {
                for (var y = 0; y < result.Height; y++)
                {
                    var cross = true;
                    
                    if (laplacianArray[x, y] == 1)
                    {
                        var x0 = Math.Max(x - 1, 0);
                        var y0 = Math.Max(y - 1, 0);
                        var x1 = x;
                        var y1 = y;
                        var x2 = Math.Min(x + 1, result.Width - 1);
                        var y2 = Math.Min(y + 1, result.Height - 1);

                        var p1 = laplacianArray[x0, y0];
                        var p2 = laplacianArray[x1, y0];
                        var p3 = laplacianArray[x2, y0];
                        var p4 = laplacianArray[x0, y1];
                        var p5 = laplacianArray[x2, y1];
                        var p6 = laplacianArray[x0, y2];
                        var p7 = laplacianArray[x1, y2];
                        var p8 = laplacianArray[x2, y2];

                        if (p1 <= -1 || p2 <= -1 || p3 <= -1 || p4 <= -1 || p5 <= -1 || p6 <= -1 || p7 <= -1)
                            cross = false;
                    }

                    result.SetPixel(x, y, cross ? Color.White : Color.Black);
                }
            }

            return result;
        }
    }
}