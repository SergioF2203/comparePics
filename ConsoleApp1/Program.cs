using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePathName = @"D:\C#\test2rows5colum.png";

            int stepPicX = 140;
            int stepPicY = 140;

            var watch = System.Diagnostics.Stopwatch.StartNew();

            Bitmap picfile = new Bitmap(filePathName);
            Bitmap bitmap = new Bitmap(picfile);

            int count = 0;
            int allcount = 0;
            int countOfComparedPictures = 0;
            int yCoordinateOfComparePictures = 0;

            for (int yComparePicture = 0; yComparePicture < bitmap.Height; yComparePicture += stepPicY)
            {
                for (int xComparedPicture = 0; xComparedPicture < bitmap.Width; xComparedPicture += stepPicX)
                {
                    for (int yNextPictures = 0; yNextPictures < bitmap.Height; yNextPictures += stepPicY)
                    {
                        for (int xNextPictures = 0; xNextPictures < bitmap.Width; xNextPictures += stepPicX)
                        {
                            if ((xNextPictures == xComparedPicture && yNextPictures == yComparePicture) ||
                                (xNextPictures < xComparedPicture && yNextPictures < yComparePicture)) continue;

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

                                if ((double)count / allcount > 0.2)
                                {
                                    Console.WriteLine($"X coordinate of compare Pic {xComparedPicture}");
                                    Console.WriteLine($"Y coordinate of compare Pic {yComparePicture}");
                                    Console.WriteLine($"X coordinate of compared Pic {xNextPictures}");
                                    Console.WriteLine($"Y coordinate of compared Pic {yNextPictures}");
                                    Console.WriteLine($"precision = {(double)count / allcount * 100}%");
                                    Console.WriteLine($"xNext = {xNextPictures}");
                                    Console.WriteLine();
                                }

                                count = 0; allcount = 0;

                            }
                        }
                    }

                }

            }

            Console.WriteLine($"work with {countOfComparedPictures} pics");
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine((double)elapsedMs / 1000);





        }
    }
}
