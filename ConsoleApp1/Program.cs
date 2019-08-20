using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePathName = ConfigurationManager.AppSettings.Get("pathFileName");
            string filePathNameOutput = ConfigurationManager.AppSettings.Get("pathOutputFileName");

            int stepPicX = 140;
            int stepPicY = 140;

            int sideSizeCompressedPicture = 16;
            float precisionIndex = 0.5f;

            var watch = System.Diagnostics.Stopwatch.StartNew();

            Bitmap picfile = new Bitmap(filePathName);
            Bitmap bitmap = new Bitmap(picfile);

            int count = 0;
            int allcount = 0;
            int countOfComparedPictures = 0;

            for (int yComparePicture = 0; yComparePicture < bitmap.Height; yComparePicture += stepPicY)
            {
                for (int xComparedPicture = 0; xComparedPicture < bitmap.Width; xComparedPicture += stepPicX)
                {
                    for (int yNextPictures = 0; yNextPictures < bitmap.Height; yNextPictures += stepPicY)
                    {
                        for (int xNextPictures = 0; xNextPictures < bitmap.Width; xNextPictures += stepPicX)
                        {
                            if ((xNextPictures == xComparedPicture && yNextPictures == yComparePicture) ||
                                (xNextPictures < xComparedPicture && yNextPictures < yComparePicture) ||
                                (xNextPictures < xComparedPicture && yNextPictures == yComparePicture) ||
                                (yNextPictures < yComparePicture)) continue;

                            if (xComparedPicture < bitmap.Width)
                            {
                                //compare with custom Hash function
                                Rectangle cloneRect = new Rectangle(xComparedPicture, yComparePicture, stepPicX, stepPicY);
                                Rectangle cloneCompareRect = new Rectangle(xNextPictures, yNextPictures, stepPicX, stepPicY);
                                System.Drawing.Imaging.PixelFormat format = bitmap.PixelFormat;
                                Bitmap cloneBitmap = bitmap.Clone(cloneRect, format);
                                Bitmap cloneCompareBitmap = bitmap.Clone(cloneCompareRect, format);

                                List<bool> iHash1 = GetHash(cloneBitmap, sideSizeCompressedPicture, precisionIndex);
                                List<bool> iHash2 = GetHash(cloneCompareBitmap, sideSizeCompressedPicture, precisionIndex);

                                cloneBitmap.Dispose();
                                cloneCompareBitmap.Dispose();

                                int equalElemets = iHash1.Zip(iHash2, (i, j) => i == j).Count(eq => eq);
                                countOfComparedPictures++;
                                if ((double)equalElemets / Math.Pow(sideSizeCompressedPicture, 2) > 0.99)
                                {
                                    Console.WriteLine($"X coordinate of compare Pic {xComparedPicture}");
                                    Console.WriteLine($"Y coordinate of compare Pic {yComparePicture}");
                                    Console.WriteLine($"X coordinate of compared Pic {xNextPictures}");
                                    Console.WriteLine($"Y coordinate of compared Pic {yNextPictures}");
                                    Console.WriteLine($"precision (equal elements) = {(double)equalElemets / Math.Pow(sideSizeCompressedPicture, 2) * 100}");
                                    Console.WriteLine();
                                }


                                //compare pic with another ones
                                //for (int xCoordinate = xComparedPicture; xCoordinate < xComparedPicture + stepPicX; xCoordinate++)
                                //{
                                //    for (int yCoordinate = yComparePicture; yCoordinate < yComparePicture + stepPicY; yCoordinate++)
                                //    {
                                //        if (bitmap.GetPixel(xCoordinate, yCoordinate).R == bitmap.GetPixel(xNextPictures + xCoordinate - xComparedPicture, yCoordinate + yNextPictures - yComparePicture).R &&
                                //            bitmap.GetPixel(xCoordinate, yCoordinate).G == bitmap.GetPixel(xNextPictures + xCoordinate - xComparedPicture, yCoordinate + yNextPictures - yComparePicture).G &&
                                //            bitmap.GetPixel(xCoordinate, yCoordinate).B == bitmap.GetPixel(xNextPictures + xCoordinate - xComparedPicture, yCoordinate + yNextPictures - yComparePicture).B)
                                //            count++;
                                //        allcount++;
                                //    }
                                //}
                                //countOfComparedPictures++;

                                //if ((double)count / allcount > 0.8 && (double)count / allcount < 0.82)
                                //{
                                //    Console.WriteLine($"X coordinate of compare Pic {xComparedPicture}");
                                //    Console.WriteLine($"Y coordinate of compare Pic {yComparePicture}");
                                //    Console.WriteLine($"X coordinate of compared Pic {xNextPictures}");
                                //    Console.WriteLine($"Y coordinate of compared Pic {yNextPictures}");
                                //    Console.WriteLine($"precision = {(double)count / allcount * 100}%");
                                //    Console.WriteLine($"xNext = {xNextPictures}");
                                //    Console.WriteLine();

                                //    //Random randomColor = new Random();
                                //    //Color color = Color.FromArgb(randomColor.Next(256), randomColor.Next(256), randomColor.Next(256));
                                //    //ColoredDublicate(bitmap, xNextPictures, yNextPictures, stepPicX, color);
                                //}
                                //count = 0; allcount = 0;
                            }
                        }
                    }
                }
            }

            //if (File.Exists(filePathNameOutput))
            //    File.Delete(filePathNameOutput);

            //bitmap.Save(filePathNameOutput);

            Console.WriteLine($"work with {countOfComparedPictures} pics");
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine((double)elapsedMs / 1000);
        }

        private static void ColoredDublicate(Bitmap bitmap, int leftUpperCornerX, int leftUpperCornerY, int sideSizeRect, Color color)
        {
            for (int i = leftUpperCornerX; i < leftUpperCornerX + sideSizeRect; i++)
            {
                //Upper horizontal line
                bitmap.SetPixel(i, leftUpperCornerY, color);
                bitmap.SetPixel(i, leftUpperCornerY + 1, color);
                bitmap.SetPixel(i, leftUpperCornerY + 2, color);

                //Right vertical line
                bitmap.SetPixel(leftUpperCornerX + sideSizeRect - 1, i + leftUpperCornerY - leftUpperCornerX, color);
                bitmap.SetPixel(leftUpperCornerX + sideSizeRect - 2, i + leftUpperCornerY - leftUpperCornerX, color);
                bitmap.SetPixel(leftUpperCornerX + sideSizeRect - 3, i + leftUpperCornerY - leftUpperCornerX, color);

                //Bottom horizontal line
                bitmap.SetPixel(i, leftUpperCornerY + sideSizeRect - 1, color);
                bitmap.SetPixel(i, leftUpperCornerY + sideSizeRect - 2, color);
                bitmap.SetPixel(i, leftUpperCornerY + sideSizeRect - 3, color);

                //Left vertical line
                bitmap.SetPixel(leftUpperCornerX, i + leftUpperCornerY - leftUpperCornerX, color);
                bitmap.SetPixel(leftUpperCornerX + 1, i + leftUpperCornerY - leftUpperCornerX, color);
                bitmap.SetPixel(leftUpperCornerX + 2, i + leftUpperCornerY - leftUpperCornerX, color);
            }
        }

        private static List<bool> GetHash(Bitmap bitmap, int size, float precision)
        {
            List<bool> IResult = new List<bool>();
            Bitmap bitmapMin = new Bitmap(bitmap, new Size(size, size));
            for (int y = 0; y < bitmapMin.Height; y++)
            {
                for (int x = 0; x < bitmapMin.Width; x++)
                {
                    IResult.Add(bitmapMin.GetPixel(x, y).GetBrightness() < precision);
                }
            }
            return IResult;
        }
    }
}
