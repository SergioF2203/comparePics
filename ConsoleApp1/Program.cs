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
            string filePathName = @"D:\C#\testpic.png";
            int stepPicX = 140;
            int stepPicY = 140;

            var watch = System.Diagnostics.Stopwatch.StartNew();
            Bitmap picfile = new Bitmap(filePathName);
            Bitmap bitmap = new Bitmap(picfile);

            int count = 0;
            int allcount = 0;
            //int step = 140;
            int countOfPic = 0;
            for (int stepY = 0; stepY < bitmap.Height;)
            {
                for (int stepX = stepPicX; stepX < bitmap.Width;)
                {
                    for (int i = 0; i < 140; i++)
                    {
                        for (int j = 0; j < 140; j++)
                        {
                            if (bitmap.GetPixel(i, j).R == bitmap.GetPixel(i + stepX, j + stepY).R &&
                                bitmap.GetPixel(i, j).G == bitmap.GetPixel(i + stepX, j + stepY).G &&
                                bitmap.GetPixel(i, j).B == bitmap.GetPixel(i + stepX, j + stepY).B)
                                count++;
                            //else
                            //{
                            //    Console.WriteLine($"x = {i}, y = {j} ");
                            //    Console.WriteLine($"R={bitmap.GetPixel(i, j).R}, " +
                            //        $"G={bitmap.GetPixel(i, j).G}, B={bitmap.GetPixel(i, j).B}, A={bitmap.GetPixel(i, j).A}");
                            //    Console.WriteLine($"R={bitmap.GetPixel(i + 280, j).R}, " +
                            //        $"G={bitmap.GetPixel(i + 280, j).G}, B={bitmap.GetPixel(i + 280, j).B}, A={bitmap.GetPixel(i + 280, j).A}");

                            //}
                            allcount++;
                        }
                    }
                    countOfPic++;
                    if ((double)count / allcount > 0.9)
                    {
                        Console.WriteLine($"Pic# {countOfPic}");
                        Console.WriteLine($"stepPic = {stepX}");
                        //Console.WriteLine($"equal pixels = {count}");
                        //Console.WriteLine($"all count = {allcount}");
                        Console.WriteLine($"precision = {(double)count / allcount * 100}%");
                        Console.WriteLine();
                    }

                    count = 0; allcount = 0; stepX += stepPicX;
                }

                stepY += stepPicY;
            }

            Console.WriteLine($"work with {countOfPic} pics");
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine((double)elapsedMs / 1000);





        }
    }
}
