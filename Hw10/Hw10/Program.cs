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

                GetZeroCrossingDetectorImage(GetLaplacianArray(image, 15, LaplacianMask1Kernel), 3, 3)?.Save(@"answers\LaplacianMask-1.png");
                GetZeroCrossingDetectorImage(GetLaplacianArray(image, 15, LaplacianMask2Kernel, 3), 3, 3)?.Save(@"answers\LaplacianMask-2.png");
                GetZeroCrossingDetectorImage(GetLaplacianArray(image, 20, MinimumVarianceLaplacianKernel, 3), 3, 3)?.Save(@"answers\MinimumVarianceLaplacian.png");
                GetZeroCrossingDetectorImage(GetLaplacianArray(image, 3000, LaplacianOfGaussiankernel), 11, 11)?.Save(@"answers\LaplacianOfGaussian.png");
                GetZeroCrossingDetectorImage(GetLaplacianArray(image, 1, DifferenceOfGaussianKernel), 11, 11)?.Save(@"answers\DifferenceOfGaussian.png");

                Console.WriteLine("complete...");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        //private static int[,] GetLaplacianMask1Array(Bitmap srcImg, int threshold)
        //{
        //    var result = new int[srcImg.Width, srcImg.Height];

        //    for (var x = 0; x < srcImg.Width; x++)
        //        for (var y = 0; y < srcImg.Height; y++)
        //        {
        //            var x0 = Math.Max(x - 1, 0);
        //            var y0 = Math.Max(y - 1, 0);
        //            var x1 = x;
        //            var y1 = y;
        //            var x2 = Math.Min(x + 1, srcImg.Width - 1);
        //            var y2 = Math.Min(y + 1, srcImg.Height - 1);

        //            var neighbors = new int[]
        //            {
        //            srcImg.GetPixel(x0, y0).R,
        //            srcImg.GetPixel(x1, y0).R,
        //            srcImg.GetPixel(x2, y0).R,
        //            srcImg.GetPixel(x0, y1).R,
        //            srcImg.GetPixel(x1, y1).R,
        //            srcImg.GetPixel(x2, y1).R,
        //            srcImg.GetPixel(x0, y2).R,
        //            srcImg.GetPixel(x1, y2).R,
        //            srcImg.GetPixel(x2, y2).R
        //            };

        //            var magnitude = 0 * neighbors[0] + 1 * neighbors[1] + 0 * neighbors[2] +
        //                            1 * neighbors[3] + -4 * neighbors[4] + 1 * neighbors[5] +
        //                            0 * neighbors[6] + 1 * neighbors[7] + 0 * neighbors[8];

        //            if (magnitude >= threshold)
        //                result[x, y] = 1;
        //            else if (magnitude <= -threshold)
        //                result[x, y] = -1;
        //            else
        //                result[x, y] = 0;
        //        }

        //    return result;
        //}

        //private static int[,] GetLaplacianMask2Array(Bitmap srcImg, int threshold)
        //{
        //    var result = new int[srcImg.Width, srcImg.Height];

        //    for (var x = 0; x < srcImg.Width; x++)
        //        for (var y = 0; y < srcImg.Height; y++)
        //        {
        //            var x0 = Math.Max(x - 1, 0);
        //            var y0 = Math.Max(y - 1, 0);
        //            var x1 = x;
        //            var y1 = y;
        //            var x2 = Math.Min(x + 1, srcImg.Width - 1);
        //            var y2 = Math.Min(y + 1, srcImg.Height - 1);

        //            var neighbors = new int[]
        //            {
        //            srcImg.GetPixel(x0, y0).R,
        //            srcImg.GetPixel(x1, y0).R,
        //            srcImg.GetPixel(x2, y0).R,
        //            srcImg.GetPixel(x0, y1).R,
        //            srcImg.GetPixel(x1, y1).R,
        //            srcImg.GetPixel(x2, y1).R,
        //            srcImg.GetPixel(x0, y2).R,
        //            srcImg.GetPixel(x1, y2).R,
        //            srcImg.GetPixel(x2, y2).R
        //            };

        //            var magnitude = 1 * neighbors[0] + 1 * neighbors[1] + 1 * neighbors[2] +
        //                            1 * neighbors[3] + -8 * neighbors[4] + 1 * neighbors[5] +
        //                            1 * neighbors[6] + 1 * neighbors[7] + 1 * neighbors[8];

        //            magnitude /= 3;

        //            if (magnitude >= threshold)
        //                result[x, y] = 1;
        //            else if (magnitude <= -threshold)
        //                result[x, y] = -1;
        //            else
        //                result[x, y] = 0;
        //        }

        //    return result;
        //}

        //private static int[,] GetMinVarianceArray(Bitmap srcImg, int threshold)
        //{
        //    var result = new int[srcImg.Width, srcImg.Height];

        //    for (var x = 0; x < srcImg.Width; x++)
        //        for (var y = 0; y < srcImg.Height; y++)
        //        {
        //            var x0 = Math.Max(x - 1, 0);
        //            var y0 = Math.Max(y - 1, 0);
        //            var x1 = x;
        //            var y1 = y;
        //            var x2 = Math.Min(x + 1, srcImg.Width - 1);
        //            var y2 = Math.Min(y + 1, srcImg.Height - 1);

        //            var neighbors = new int[]
        //            {
        //            srcImg.GetPixel(x0, y0).R,
        //            srcImg.GetPixel(x1, y0).R,
        //            srcImg.GetPixel(x2, y0).R,
        //            srcImg.GetPixel(x0, y1).R,
        //            srcImg.GetPixel(x1, y1).R,
        //            srcImg.GetPixel(x2, y1).R,
        //            srcImg.GetPixel(x0, y2).R,
        //            srcImg.GetPixel(x1, y2).R,
        //            srcImg.GetPixel(x2, y2).R
        //            };

        //            var magnitude = 2 * neighbors[0] + -1 * neighbors[1] + 2 * neighbors[2] +
        //                            -1 * neighbors[3] + -4 * neighbors[4] + -1 * neighbors[5] +
        //                            2 * neighbors[6] + -1 * neighbors[7] + 2 * neighbors[8];
        //            magnitude /= 3;

        //            if (magnitude >= threshold)
        //                result[x, y] = 1;
        //            else if (magnitude <= -threshold)
        //                result[x, y] = -1;
        //            else
        //                result[x, y] = 0;
        //        }

        //    return result;
        //}

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
        /// 取得 Zero Crossing
        /// </summary>
        /// <param name="gradient"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private static Bitmap GetZeroCrossingDetectorImage(int[,] gradient, int width, int height)
        {
            var result = new Bitmap(gradient.GetLength(0), gradient.GetLength(1));

            for (var x = 0; x < result.Width; x++)
                for (var y = 0; y < result.Height; y++)
                {
                    var cross = 1;
                    if (gradient[x, y] == 1)
                        for (var x1 = -width / 2; x1 <= width / 2 + 1; x1++)
                            for (var y1 = -height / 2; y1 <= height / 2 + 1; y1++)
                            {
                                var destX = x + x1 < 0 ? 0 : x + x1;
                                destX = destX > result.Width - 1 ? result.Width - 1 : destX;
                                var destY = y + y1 < 0 ? 0 : y + y1;
                                destY = destY > result.Height - 1 ? result.Height - 1 : destY;

                                if (gradient[destX, destY] == -1)
                                    cross = 0;
                            }

                    result.SetPixel(x, y, cross == 1 ? Color.White : Color.Black);
                }

            return result;
        }

        //private static int[,] GetLaplacianOfGaussianArray1(Bitmap srcImg, int threshold)
        //{
        //    var kernel = new[]
        //    {
        //        new[] {0, 0, 0, -1, -1, -2, -1, -1, 0, 0, 0},
        //        new[] {0, 0, -2, -4, -8, -9, -8, -4, -2, 0, 0},
        //        new[] {0, -2, -7, -15, -22, -23, -22, -15, -7, -2, 0},
        //        new[] {-1, -4, -15, -24, -14, -1, -14, -24, -15, -4, -1},
        //        new[] {-1, -8, -22, -14, 52, 103, 52, -14, -22, -8, -1},
        //        new[] {-2, -9, -23, -1, 103, 178, 103, -1, -23, -9, -2},
        //        new[] {-1, -8, -22, -14, 52, 103, 52, -14, -22, -8, -1},
        //        new[] {-1, -4, -15, -24, -14, -1, -14, -24, -15, -4, -1},
        //        new[] {0, -2, -7, -15, -22, -23, -22, -15, -7, -2, 0},
        //        new[] {0, 0, -2, -4, -8, -9, -8, -4, -2, 0, 0},
        //        new[] {0, 0, 0, -1, -1, -2, -1, -1, 0, 0, 0}
        //    };

        //    var result = new int[srcImg.Width, srcImg.Height];

        //    for (var x = 0; x < srcImg.Width; x++)
        //        for (var y = 0; y < srcImg.Height; y++)
        //        {
        //            var x1 = new int[11];
        //            var y1 = new int[11];

        //            for (var index = 0; index < 11; index++)
        //            {
        //                x1[index] = x + index - 5 < 0 ? 0 : x + index - 5;
        //                x1[index] = x1[index] > srcImg.Width - 1 ? srcImg.Width - 1 : x1[index];
        //                y1[index] = y + index - 5 < 0 ? 0 : y + index - 5;
        //                y1[index] = y1[index] > srcImg.Height - 1 ? srcImg.Height - 1 : y1[index];
        //            }

        //            var neighbors = new int[11, 11];

        //            for (var i = 0; i < 11; i++)
        //            {
        //                for (var j = 0; j < 11; j++)
        //                {
        //                    neighbors[i, j] = srcImg.GetPixel(x1[i], y1[j]).R;
        //                }
        //            }

        //            var magnitude = 0;
        //            for (var i = 0; i < 11; i++)
        //            {
        //                for (var j = 0; j < 11; j++)
        //                {
        //                    magnitude += kernel[i][j] * neighbors[i, j];
        //                }
        //            }

        //            if (magnitude >= threshold)
        //                result[x, y] = 1;
        //            else if (magnitude <= -threshold)
        //                result[x, y] = -1;
        //            else
        //                result[x, y] = 0;
        //        }

        //    return result;
        //}
    }
}