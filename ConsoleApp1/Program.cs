using System;
using System.Collections.Generic;
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
            string filePathName = @"D:\C#\test2rows.png";
            string filePathNameOutput = @"D:\C#\testColor.png";

            int stepPicX = 140;
            int stepPicY = 140;

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
                                for (int xCoordinate = xComparedPicture; xCoordinate < xComparedPicture + stepPicX; xCoordinate++)
                                {
                                    for (int yCoordinate = yComparePicture; yCoordinate < yComparePicture + stepPicY; yCoordinate++)
                                    {
                                        if (bitmap.GetPixel(xCoordinate, yCoordinate).R == bitmap.GetPixel(xNextPictures + xCoordinate - xComparedPicture, yCoordinate + yNextPictures - yComparePicture).R &&
                                            bitmap.GetPixel(xCoordinate, yCoordinate).G == bitmap.GetPixel(xNextPictures + xCoordinate - xComparedPicture, yCoordinate + yNextPictures - yComparePicture).G &&
                                            bitmap.GetPixel(xCoordinate, yCoordinate).B == bitmap.GetPixel(xNextPictures + xCoordinate - xComparedPicture, yCoordinate + yNextPictures - yComparePicture).B)
                                            count++;
                                        allcount++;
                                    }
                                }
                                countOfComparedPictures++;

                                if ((double)count / allcount > 0.8 && (double)count / allcount < 0.82)
                                {
                                    Console.WriteLine($"X coordinate of compare Pic {xComparedPicture}");
                                    Console.WriteLine($"Y coordinate of compare Pic {yComparePicture}");
                                    Console.WriteLine($"X coordinate of compared Pic {xNextPictures}");
                                    Console.WriteLine($"Y coordinate of compared Pic {yNextPictures}");
                                    Console.WriteLine($"precision = {(double)count / allcount * 100}%");
                                    Console.WriteLine($"xNext = {xNextPictures}");
                                    Console.WriteLine();

                                    //Random randomColor = new Random();
                                    //Color color = Color.FromArgb(randomColor.Next(256), randomColor.Next(256), randomColor.Next(256));
                                    //ColoredDublicate(bitmap, xNextPictures, yNextPictures, stepPicX, color);
                                }
                                count = 0; allcount = 0;
                            }
                        }
                    }
                }
            }

            //ColoredDublicate(bitmap, 560, 140, stepPicX);

            if (File.Exists(filePathNameOutput))
                File.Delete(filePathNameOutput);

            bitmap.Save(filePathNameOutput);

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

        private static List<bool> GetHash(Bitmap bitmap)
        {
            List<bool> IResult = new List<bool>();
            Bitmap bitmapMin = new Bitmap(bitmap, new Size(16, 16));
            for(int y = 0; y<bitmapMin.Height; y++)
            {
                for(int x = 0; x < bitmapMin.Width; x++)
                {
                    IResult.Add(bitmapMin.GetPixel(x, y).GetBrightness() < 0.5f);
                }
            }
            return IResult;
        }


    }
}
