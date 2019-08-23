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

            List<Point> coordinatesComparedPictures = new List<Point>();

            int stepPicX = 140;
            int stepPicY = 140;

            int sideSizeCompressedPicture = 8;
            int powTwoSizeCompressedPicture = sideSizeCompressedPicture * sideSizeCompressedPicture;
            double precisionPercent = 0.965;

            int countOfComparedPictures = 0;

            bool dublicate = false;

            Dictionary<Point, List<bool>> dictionaryPicHash = new Dictionary<Point, List<bool>>();

            var watch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                using (Bitmap picfile = new Bitmap(filePathName))
                {
                    Bitmap bitmap = new Bitmap(picfile);
                    if (IsImage(bitmap))
                    {
                        float precisionIndex = StandartDeviationBrightness(bitmap);

                        for (int yComparePicture = 0; yComparePicture < bitmap.Height; yComparePicture += stepPicY)
                        {
                            for (int xComparedPicture = 0; xComparedPicture < bitmap.Width; xComparedPicture += stepPicX)
                            {
                                Bitmap singlePicture = PurePicture(bitmap, xComparedPicture, yComparePicture, stepPicX, stepPicY);
                                dictionaryPicHash.Add(new Point(xComparedPicture, yComparePicture), GetHash(singlePicture, sideSizeCompressedPicture, precisionIndex));

                                //Console.WriteLine($"Pic# {yComparePicture / 140 * bitmap.Width / 140 + xComparedPicture / 140 + 1}");
                                //Console.WriteLine($"Pics Num = {dictionaryPicHash.Count}");

                                //the same pics for don't compare
                                //foreach (Point coordinate in coordinatesComparedPictures)
                                //{
                                //    if (coordinate.X == xComparedPicture && coordinate.Y == yComparePicture)
                                //    {
                                //        dublicate = true;
                                //        break;
                                //    }
                                //    dublicate = false;
                                //}

                                //if (!dublicate)
                                //{
                                //    Random randomColor = new Random();
                                //    Color color = Color.FromArgb(randomColor.Next(256), randomColor.Next(256), randomColor.Next(256));
                                //    Bitmap toComparePicture = PurePicture(bitmap, xComparedPicture, yComparePicture, stepPicX, stepPicY);

                                //    for (int yNextPictures = 0; yNextPictures < bitmap.Height; yNextPictures += stepPicY)
                                //    {
                                //        for (int xNextPictures = 0; xNextPictures < bitmap.Width; xNextPictures += stepPicX)
                                //        {
                                //            if ((xNextPictures == xComparedPicture && yNextPictures == yComparePicture) ||
                                //                (xNextPictures < xComparedPicture && yNextPictures < yComparePicture) ||
                                //                (xNextPictures < xComparedPicture && yNextPictures == yComparePicture) ||
                                //                (yNextPictures < yComparePicture)) continue;

                                //            if (xComparedPicture < bitmap.Width)
                                //            {

                                //                     //Bitmap comparedPicture = PurePicture(bitmap, xNextPictures, yNextPictures, stepPicX, stepPicY);
                                //                //if (ComparsionByHash(toComparePicture, comparedPicture, sideSizeCompressedPicture, precisionIndex) > precisionPercent)
                                //                //{
                                //                //    ColoredDublicate(bitmap, xComparedPicture, yComparePicture, stepPicX, color);
                                //                //    ColoredDublicate(bitmap, xNextPictures, yNextPictures, stepPicX, color);

                                //                //    coordinatesComparedPictures.Add(new Point(xNextPictures, yNextPictures));
                                //                //}
                                //                //countOfComparedPictures++;
                                //            }
                                //        }
                                //    }
                                //}
                            }
                        }


                        foreach (KeyValuePair<Point, List<bool>> pair in dictionaryPicHash)
                        {
                            Random randomColor = new Random();
                            Color color = Color.FromArgb(randomColor.Next(256), randomColor.Next(256), randomColor.Next(256));

                            foreach (KeyValuePair<Point, List<bool>> subPair in dictionaryPicHash)
                            {
                                //Console.WriteLine((double)ComparsionOnlyHash(pair.Value, subPair.Value) / powTwoSizeCompressedPicture);

                                if (pair.Key == subPair.Key)
                                    continue;
                                else if(((double)ComparsionOnlyHash(pair.Value, subPair.Value) / powTwoSizeCompressedPicture) > precisionPercent)
                                {
                                    ColoredDublicate(bitmap, pair.Key.X, pair.Key.Y, stepPicX, color);
                                    ColoredDublicate(bitmap, subPair.Key.X, subPair.Key.Y, stepPicX, color);
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
            }
            catch (Exception ex)
            {
                //Console.WriteLine("The file does not have a valid image format");
                Console.WriteLine(ex.Message);
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

        private static Rectangle ImageRectangle(Bitmap _bitmap)
        {
            int leftUpperCornerX = 0;
            int leftUpperCornerY = 0;
            int rightDownCornerX = 0;
            int rightDownCornerY = 0;

            for (int y = 0; y < _bitmap.Height; y++)
            {
                for (int x = 0; x < _bitmap.Width; x++)
                {
                    if (_bitmap.GetPixel(x, y).R != 0 || _bitmap.GetPixel(x, y).G != 0 || _bitmap.GetPixel(x, y).B != 0 || _bitmap.GetPixel(x, y).A != 0)
                    {
                        leftUpperCornerY = y;
                        y = _bitmap.Height;
                        break;
                    }
                }
            }

            for (int x = 0; x < _bitmap.Width; x++)
            {
                for (int y = 0; y < _bitmap.Height; y++)
                {
                    if (_bitmap.GetPixel(x, y).R != 0 || _bitmap.GetPixel(x, y).G != 0 || _bitmap.GetPixel(x, y).B != 0 || _bitmap.GetPixel(x, y).A != 0)
                    {
                        leftUpperCornerX = x;
                        x = _bitmap.Width;
                        break;
                    }

                }
            }

            for (int y = _bitmap.Height - 1; y > 0; y--)
            {
                for (int x = _bitmap.Width - 1; x > 0; x--)
                {
                    if (_bitmap.GetPixel(x, y).R != 0 || _bitmap.GetPixel(x, y).G != 0 || _bitmap.GetPixel(x, y).B != 0 || _bitmap.GetPixel(x, y).A != 0)
                    {
                        rightDownCornerY = y + 1;
                        y = 0;
                        break;
                    }

                }
            }

            for (int x = _bitmap.Width - 1; x > 0; x--)
            {
                for (int y = _bitmap.Height - 1; y > 0; y--)
                {
                    if (_bitmap.GetPixel(x, y).R != 0 || _bitmap.GetPixel(x, y).G != 0 || _bitmap.GetPixel(x, y).B != 0 || _bitmap.GetPixel(x, y).A != 0)
                    {
                        rightDownCornerX = x + 1;
                        x = 0;
                        break;
                    }

                }
            }

            if (rightDownCornerX - leftUpperCornerX == 0 || rightDownCornerY - leftUpperCornerY == 0)
                return new Rectangle(leftUpperCornerX, leftUpperCornerY, 10, 10);

            return new Rectangle(leftUpperCornerX, leftUpperCornerY, rightDownCornerX - leftUpperCornerX, rightDownCornerY - leftUpperCornerY);
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

        private static Bitmap PurePicture(Bitmap _bitmap, int _xCoordinate, int _yCoordinate, int _stepPicX, int _stepPicY)
        {
            Rectangle cloneRect = new Rectangle(_xCoordinate, _yCoordinate, _stepPicX, _stepPicY);

            System.Drawing.Imaging.PixelFormat format = _bitmap.PixelFormat;

            Bitmap cloneBitmap = _bitmap.Clone(cloneRect, format);

            Rectangle firstRect = ImageRectangle(cloneBitmap);

            Bitmap purePicure = cloneBitmap.Clone(firstRect, format);

            return purePicure;
        }

        private static int ComparsionOnlyHash(List<bool> hash1, List<bool> hash2)
        {
            return hash1.Zip(hash2, (i, j) => i == j).Count(eq => eq);
        }

        private static double ComparsionByHash(Bitmap _bitmap1, Bitmap _bitmap2, int _sideSizeCompressedPicture, float _precisionIndex)
        {
            List<bool> iHash1 = GetHash(_bitmap1, _sideSizeCompressedPicture, _precisionIndex);
            List<bool> iHash2 = GetHash(_bitmap2, _sideSizeCompressedPicture, _precisionIndex);

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
                    {
                        count++;
                    }

                    allcount++;
                }
            }
            Console.WriteLine($"precision: {(double)count / allcount}");
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

        private static float StandartDeviationBrightness(Bitmap _bitmap)
        {
            float summBrightness = 0f;
            int countNotNullBrightness = 0;
            double valueDeviat;
            double standartDeviation = 0;
            double numerator = 0;

            for (int i = 0; i < _bitmap.Width; i++)
            {
                for (int j = 0; j < _bitmap.Height; j++)
                {
                    if (_bitmap.GetPixel(i, j).GetBrightness() != 0)
                        countNotNullBrightness++;
                    summBrightness += _bitmap.GetPixel(i, j).GetBrightness();
                }
            }

            valueDeviat = summBrightness / (_bitmap.Width * _bitmap.Height);
            float medianBrightness = summBrightness / countNotNullBrightness;

            for (int i = 0; i < _bitmap.Width; i++)
            {
                for (int j = 0; j < _bitmap.Height; j++)
                {
                    numerator += Math.Pow((_bitmap.GetPixel(i, j).GetBrightness() - valueDeviat), 2);
                }
            }

            standartDeviation = Math.Sqrt(numerator / (_bitmap.Width * _bitmap.Height));


            //for (int i = 0; i < _bitmap.Width; i++)
            //{
            //    for (int j = 0; j < _bitmap.Height; j++)
            //    {
            //        if (_bitmap.GetPixel(i, j).GetBrightness() < medianBrightness)
            //        {
            //            Color color = Color.FromArgb(_bitmap.GetPixel(i, j).R * (int)(standartDeviation / _bitmap.GetPixel(i, j).GetBrightness() + _bitmap.GetPixel(i, j).R),
            //                _bitmap.GetPixel(i, j).G * (int)(standartDeviation / _bitmap.GetPixel(i, j).GetBrightness() + _bitmap.GetPixel(i, j).G),
            //                _bitmap.GetPixel(i, j).B * (int)(standartDeviation / _bitmap.GetPixel(i, j).GetBrightness() + _bitmap.GetPixel(i, j).B));
            //            _bitmap.SetPixel(i, j, color);
            //        }
            //    }
            //}

            //return standartDeviation;
            //return valueDeviat;
            return medianBrightness;
            //return _bitmap;


        }

    }
}
