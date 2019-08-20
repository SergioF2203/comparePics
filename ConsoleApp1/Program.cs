using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;

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
            double precisionPercent = 0.999;

            var watch = System.Diagnostics.Stopwatch.StartNew();

            Bitmap picfile = new Bitmap(filePathName);
            Bitmap bitmap = new Bitmap(picfile);

            int countOfComparedPictures = 0;


            if (IsImage(bitmap))
            {
                for (int yComparePicture = 0; yComparePicture < bitmap.Height; yComparePicture += stepPicY)
                {
                    for (int xComparedPicture = 0; xComparedPicture < bitmap.Width; xComparedPicture += stepPicX)
                    {
                        Random randomColor = new Random();
                        Color color = Color.FromArgb(randomColor.Next(256), randomColor.Next(256), randomColor.Next(256));

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
                                    if (ComparsionByHash(bitmap, xComparedPicture, yComparePicture, stepPicX, stepPicY, xNextPictures, yNextPictures, sideSizeCompressedPicture, precisionIndex) > precisionPercent)
                                    {
                                        ColoredDublicate(bitmap, xComparedPicture, yComparePicture, stepPicX, color);
                                        ColoredDublicate(bitmap, xNextPictures, yNextPictures, stepPicX, color);
                                    }
                                    countOfComparedPictures++;
                                }
                            }
                        }
                    }
                }

                if (File.Exists(filePathNameOutput))
                    File.Delete(filePathNameOutput);

                bitmap.Save(filePathNameOutput);

                Console.WriteLine($"Compared {countOfComparedPictures} pics");
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine($"Elasped time: {(double)elapsedMs / 1000} sec.");
            }
            else
            {
                Console.WriteLine("File isn't an Image");
            }

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

        private static double ComparsionByHash(Bitmap _bitmap, int _xComparedPicture, int _yComparePicture,
            int _stepPicX, int _stepPicY, int _xNextPictures, int _yNextPictures, int _sideSizeCompressedPicture, float _precisionIndex)
        {
            Rectangle cloneRect = new Rectangle(_xComparedPicture, _yComparePicture, _stepPicX, _stepPicY);
            Rectangle cloneCompareRect = new Rectangle(_xNextPictures, _yNextPictures, _stepPicX, _stepPicY);
            System.Drawing.Imaging.PixelFormat format = _bitmap.PixelFormat;
            Bitmap cloneBitmap = _bitmap.Clone(cloneRect, format);
            Bitmap cloneCompareBitmap = _bitmap.Clone(cloneCompareRect, format);

            List<bool> iHash1 = GetHash(cloneBitmap, _sideSizeCompressedPicture, _precisionIndex);
            List<bool> iHash2 = GetHash(cloneCompareBitmap, _sideSizeCompressedPicture, _precisionIndex);

            cloneBitmap.Dispose();
            cloneCompareBitmap.Dispose();

            int equalElemets = iHash1.Zip(iHash2, (i, j) => i == j).Count(eq => eq);

            return (double)equalElemets / (_sideSizeCompressedPicture * _sideSizeCompressedPicture);
        }

        private static double ComparsionByPixels(Bitmap _bitmap, int _xComparedPicture, int _yComparePicture, int _stepPicX, int _stepPicY, int _xNextPictures, int _yNextPictures)
        {
            int count = 0, allcount = 0;

            for (int xCoordinate = _xComparedPicture; xCoordinate < _xComparedPicture + _stepPicX; xCoordinate++)
            {
                for (int yCoordinate = _yComparePicture; yCoordinate < _yComparePicture + _stepPicY; yCoordinate++)
                {
                    if (_bitmap.GetPixel(xCoordinate, yCoordinate).R == _bitmap.GetPixel(_xNextPictures + xCoordinate - _xComparedPicture, yCoordinate + _yNextPictures - _yComparePicture).R &&
                        _bitmap.GetPixel(xCoordinate, yCoordinate).G == _bitmap.GetPixel(_xNextPictures + xCoordinate - _xComparedPicture, yCoordinate + _yNextPictures - _yComparePicture).G &&
                        _bitmap.GetPixel(xCoordinate, yCoordinate).B == _bitmap.GetPixel(_xNextPictures + xCoordinate - _xComparedPicture, yCoordinate + _yNextPictures - _yComparePicture).B)
                        count++;
                    allcount++;
                }
            }

            return (double)count / allcount;

        }

        private static byte[] ConvertImageToBytesArray(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

        private static bool IsImage(Image img)
        {
            var png = new byte[] { 137, 80, 78, 71 };
            var jpeg = new byte[] { 255, 216, 255, 224 };

            if (png.SequenceEqual(ConvertImageToBytesArray(img).Take(png.Length)))
                return true;
            if (jpeg.SequenceEqual(ConvertImageToBytesArray(img).Take(jpeg.Length)))
                return true;
            return false;
        }
    }
}
