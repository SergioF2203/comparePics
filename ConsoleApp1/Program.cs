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
            int xCoordinateOfComparedPictures = 0;
            int yCoordinateOfComparedPictures = 0;
            int xCoordinateOfComparePictures = 0;
            int yCoordinateOfComparePictures = 0;

            for (int xComparedPicture = 0; xComparedPicture < bitmap.Width; xComparedPicture += stepPicX)
            {
                for (int yNextPictures = 0; yNextPictures < bitmap.Height; yNextPictures += stepPicY)
                {
                    for (int xNextPictures = 0; xNextPictures < bitmap.Width; xNextPictures += stepPicX)
                    {
                        if ((xNextPictures == xComparedPicture && yNextPictures == 0) || 
                            (xNextPictures<xComparedPicture && yNextPictures == 0)) continue;

                        if (xComparedPicture < bitmap.Width)
                        {
                            for (int xCoordinate = 0; xCoordinate < stepPicX; xCoordinate++)
                            {
                                for (int yCoordinate = 0; yCoordinate < 140; yCoordinate++)
                                {
                                    if (bitmap.GetPixel(xCoordinate, yCoordinate).R == bitmap.GetPixel(xCoordinate + xNextPictures, yCoordinate + yNextPictures).R &&
                                        bitmap.GetPixel(xCoordinate, yCoordinate).G == bitmap.GetPixel(xCoordinate + xNextPictures, yCoordinate + yNextPictures).G &&
                                        bitmap.GetPixel(xCoordinate, yCoordinate).B == bitmap.GetPixel(xCoordinate + xNextPictures, yCoordinate + yNextPictures).B)
                                        count++;
                                    allcount++;
                                    yCoordinateOfComparedPictures = yCoordinate + 1 - 140;
                                    yCoordinateOfComparePictures = yCoordinate + 1 - 140;
                                }
                                xCoordinateOfComparedPictures = xCoordinate + xNextPictures - stepPicX + 1;
                                xCoordinateOfComparePictures = xCoordinate + 1 - stepPicX;
                            }
                            countOfComparedPictures++;

                            if ((double)count / allcount > 0.1)
                            {
                                Console.WriteLine($"X coordinate of compare Pic {xComparedPicture}");
                                Console.WriteLine($"Y coordinate of compare Pic {yCoordinateOfComparePictures}");
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

            Console.WriteLine($"work with {countOfComparedPictures} pics");
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine((double)elapsedMs / 1000);





        }
    }
}
