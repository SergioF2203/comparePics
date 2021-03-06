﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            //string filePathName = ConfigurationManager.AppSettings.Get("pathFileName");
            //string filePathNameOutput = ConfigurationManager.AppSettings.Get("pathOutputFileName");
            //string fileColoredPathName = ConfigurationManager.AppSettings.Get("pathColoredFileName");

            List<Point> coordinatesComparedPictures = new List<Point>();

            int stepPicX = int.Parse(ConfigurationManager.AppSettings.Get("stepPicX"));
            int stepPicY = int.Parse(ConfigurationManager.AppSettings.Get("stepPicY"));

            int sideSizeCompressedPicture = 8;
            int powTwoSizeCompressedPicture = sideSizeCompressedPicture * sideSizeCompressedPicture;
            double precisionPercent = Convert.ToDouble(ConfigurationManager.AppSettings.Get("precision")) / 100;

            int proportionate = 0;

            Dictionary<Point, List<bool>> dictionaryPicHash = new Dictionary<Point, List<bool>>();
            Dictionary<Point, List<bool>> dictionaryPicHash90 = new Dictionary<Point, List<bool>>();
            Dictionary<Point, List<bool>> dictionaryPicHash180 = new Dictionary<Point, List<bool>>();
            Dictionary<Point, List<bool>> dictionaryPicHash270 = new Dictionary<Point, List<bool>>();
            Dictionary<Point, List<bool>> dictionaryPicHashFlip = new Dictionary<Point, List<bool>>();
            Dictionary<Point, List<bool>> dictionaryPicHashVFlip = new Dictionary<Point, List<bool>>();

            Dictionary<Point, Bitmap> dictionaryUniquePics = new Dictionary<Point, Bitmap>();

            Dictionary<Point, Size> dictionaryOfSizes = new Dictionary<Point, Size>();
            List<Point> listOfPointComparedPic = new List<Point>();

            HashSet<Point> setOfDupsPic = new HashSet<Point>();
            HashSet<Point> setComparedPic = new HashSet<Point>();
            HashSet<Point> setAllPic = new HashSet<Point>();
            HashSet<Point> setEmptyPic = new HashSet<Point>();

            HashSet<Point> setBorderedPictures = new HashSet<Point>();
            HashSet<Point> setUnborderedPictures = new HashSet<Point>();

            string filePathName = "";
            string filePathNameOutput = "";
            string fileColoredPathName = "";
            int choiseOfStage = 0;
            try
            {
            Console.Write("What stage do you need? (1 - to detect unique pics stage, 2 - colored dups from unique pics stage): ");
            choiseOfStage = int.Parse(Console.ReadLine());
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                System.Environment.Exit(0);
            }

            Console.Write("Please input path and file name original picture (i.e. diskName:\\folder\\folder\\...\\picName.png): ");
            filePathName = Console.ReadLine();

            switch (choiseOfStage)
            {
                case 1:
                    Console.Write("Please input path and file name output picture (i.e. diskName:\\folder\\folder\\...\\picName.png): ");
                    filePathNameOutput = Console.ReadLine();
                    break;
                case 2:
                    Console.Write("Please input path and file name colored picture (i.e. diskName:\\folder\\folder\\...\\picName.png): ");
                    fileColoredPathName = Console.ReadLine();

                    Console.Write("Please input path and file name output picture (i.e. diskName:\\folder\\folder\\...\\picName.png): ");
                    filePathNameOutput = Console.ReadLine();
                    break;
                default:
                    break;
            }


            var watch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                using (Bitmap picfile = new Bitmap(filePathName))
                {
                    Bitmap bitmap = new Bitmap(picfile);

                    if (IsImage(bitmap))
                    {
                        //check size of image and crop
                        if (bitmap.Height % stepPicY != 0 || bitmap.Width % stepPicX != 0)
                        {
                            Console.Write("The Image has invalid size, do you want to crop it? (Y/N): ");
                            var choise = Console.ReadLine();
                            switch (choise)
                            {
                                case "Y":
                                case "y":
                                    bitmap = Extensions.Crop(bitmap, new Rectangle(0, 0, Math.Abs(bitmap.Width / stepPicX) * stepPicX, Math.Abs(bitmap.Height / stepPicY) * stepPicY));
                                    Console.WriteLine($"new Picture Width: {Math.Abs(bitmap.Width / stepPicX) * stepPicX}");
                                    Console.WriteLine($"new Picture Height: {Math.Abs(bitmap.Height / stepPicY) * stepPicY}");

                                    break;
                                case "N":
                                case "n":
                                    throw new Exception("Invalid a picture size");
                                default:
                                    break;
                            }
                            //Console.WriteLine(Math.Abs(bitmap.Height / stepPicY) * stepPicY);
                            //Console.WriteLine(Math.Abs(bitmap.Width / stepPicX) * stepPicX);

                            //Console.ReadLine();
                        }

                        float precisionIndex = StandartDeviationBrightness(bitmap);

                        for (int yComparePicture = 0; yComparePicture < bitmap.Height; yComparePicture += stepPicY)
                        {
                            for (int xComparedPicture = 0; xComparedPicture < bitmap.Width; xComparedPicture += stepPicX)
                            {
                                Bitmap singlePicture = PurePicture(bitmap, xComparedPicture, yComparePicture, stepPicX, stepPicY);

                                setAllPic.Add(new Point(xComparedPicture, yComparePicture));

                                dictionaryPicHash.Add(new Point(xComparedPicture, yComparePicture), GetWholeHash(singlePicture, 0));
                                dictionaryPicHash90.Add(new Point(xComparedPicture, yComparePicture), GetWholeHash(singlePicture, 90));
                                dictionaryPicHash180.Add(new Point(xComparedPicture, yComparePicture), GetWholeHash(singlePicture, 180));
                                dictionaryPicHash270.Add(new Point(xComparedPicture, yComparePicture), GetWholeHash(singlePicture, 270));
                                dictionaryPicHashFlip.Add(new Point(xComparedPicture, yComparePicture), GetWholeHash(singlePicture, 180180));
                                dictionaryPicHashVFlip.Add(new Point(xComparedPicture, yComparePicture), GetWholeHash(singlePicture, 270270));

                                dictionaryOfSizes.Add(new Point(xComparedPicture, yComparePicture), new Size(singlePicture.Width, singlePicture.Height));
                            }
                        }

                        foreach (KeyValuePair<Point, Size> item in dictionaryOfSizes)
                        {
                            if (IsExist(listOfPointComparedPic, item.Key))
                                continue;

                            foreach (KeyValuePair<Point, Size> subItem in dictionaryOfSizes)
                            {
                                if (subItem.Key == item.Key)
                                    continue;

                                if (IsExist(listOfPointComparedPic, subItem.Key))
                                    continue;

                                if (item.Key.Y > subItem.Key.Y || (item.Key.Y == subItem.Key.Y && item.Key.X > subItem.Key.X))
                                    continue;

                                proportionate = IsProportionate(item.Value, subItem.Value);

                                if (subItem.Value.Width != 1 && subItem.Value.Height != 1)
                                {
                                    if (proportionate != 0)
                                    {
                                        if (CompareHashs(dictionaryPicHash[item.Key], dictionaryPicHash180[subItem.Key]) > precisionPercent ||
                                            CompareHashs(dictionaryPicHash[item.Key], dictionaryPicHash[subItem.Key]) > precisionPercent ||
                                            CompareHashs(dictionaryPicHash[item.Key], dictionaryPicHashFlip[subItem.Key]) > precisionPercent ||
                                            CompareHashs(dictionaryPicHash[item.Key], dictionaryPicHash90[subItem.Key]) > precisionPercent ||
                                            CompareHashs(dictionaryPicHash[item.Key], dictionaryPicHash270[subItem.Key]) > precisionPercent ||
                                            CompareHashs(dictionaryPicHash[item.Key], dictionaryPicHashVFlip[subItem.Key]) > precisionPercent)
                                        {
                                            AddPointsToHashsetList(setOfDupsPic, setComparedPic, item.Key, subItem.Key, listOfPointComparedPic);
                                        }
                                    }
                                }
                                else
                                    setEmptyPic.Add(subItem.Key);
                            }
                        }

                        setAllPic.ExceptWith(setEmptyPic);
                        int countAllPics = setAllPic.Count;

                        setAllPic.ExceptWith(setComparedPic);
                        int countUniquePic = setAllPic.Count;

                        switch (choiseOfStage)
                        {
                            case 1:
                                foreach (var item in setAllPic)
                                {
                                    //ColoredDublicate(bitmap, item.X, item.Y, stepPicX, Color.FromArgb(0, 255, 0));

                                    Brush brush = new SolidBrush(Color.FromArgb(0, 255, 0));
                                    ColoredDublicateMarkCorner(bitmap, item.X, item.Y, brush);

                                }

                                if (File.Exists(filePathNameOutput))
                                    File.Delete(filePathNameOutput);

                                bitmap.Save(filePathNameOutput);

                                break;
                            case 2:
                                Bitmap coloredBmp = new Bitmap(fileColoredPathName);
                                Bitmap coloredBitmap = new Bitmap(coloredBmp);

                                coloredBitmap.MakeTransparent(Color.FromArgb(0, 255, 0));

                                foreach (var item in setAllPic)
                                {
                                    foreach (var subItem in dictionaryOfSizes)
                                    {
                                        if (CompareHashs(dictionaryPicHash[item], dictionaryPicHash[subItem.Key]) > precisionPercent)
                                        {
                                            TransformPicture(coloredBitmap, item.X, item.Y, stepPicX, stepPicY, subItem.Key.X, subItem.Key.Y, RotateFlipType.Rotate180FlipXY);
                                        }
                                        else if (CompareHashs(dictionaryPicHash[item], dictionaryPicHash90[subItem.Key]) > precisionPercent)
                                        {
                                            TransformPicture(coloredBitmap, item.X, item.Y, stepPicX, stepPicY, subItem.Key.X, subItem.Key.Y, RotateFlipType.Rotate90FlipNone);
                                        }
                                        else if (CompareHashs(dictionaryPicHash[item], dictionaryPicHash180[subItem.Key]) > precisionPercent)
                                        {
                                            TransformPicture(coloredBitmap, item.X, item.Y, stepPicX, stepPicY, subItem.Key.X, subItem.Key.Y, RotateFlipType.Rotate180FlipNone);
                                        }
                                        else if (CompareHashs(dictionaryPicHash[item], dictionaryPicHash270[subItem.Key]) > precisionPercent)
                                        {
                                            TransformPicture(coloredBitmap, item.X, item.Y, stepPicX, stepPicY, subItem.Key.X, subItem.Key.Y, RotateFlipType.Rotate270FlipNone);
                                        }
                                        else if (CompareHashs(dictionaryPicHash[item], dictionaryPicHashFlip[subItem.Key]) > precisionPercent)
                                        {
                                            TransformPicture(coloredBitmap, item.X, item.Y, stepPicX, stepPicY, subItem.Key.X, subItem.Key.Y, RotateFlipType.RotateNoneFlipX);
                                        }
                                        else if (CompareHashs(dictionaryPicHash[item], dictionaryPicHashVFlip[subItem.Key]) > precisionPercent)
                                        {
                                            TransformPicture(coloredBitmap, item.X, item.Y, stepPicX, stepPicY, subItem.Key.X, subItem.Key.Y, RotateFlipType.RotateNoneFlipY);
                                        }
                                    }
                                }

                                if (File.Exists(filePathNameOutput))
                                    File.Delete(filePathNameOutput);

                                coloredBitmap.Save(filePathNameOutput);
                                break;
                            default:
                                break;
                        }

                        Console.WriteLine($"Count of Pics: {countAllPics}");
                        Console.WriteLine($"Dubs Pic: {setOfDupsPic.Count} pics");
                        Console.WriteLine($"Associated with dups pics: {setComparedPic.Count}");
                        Console.WriteLine($"Unique pics: {countUniquePic}");

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
                Console.WriteLine(ex.Message);
            }

            Console.ReadLine();
        }


        private static Bitmap WholePicture(Bitmap _bitmap, int _xCoordinate, int _yCoordinate, int _stepPicX, int _stepPicY)
        {
            Rectangle cloneRect = new Rectangle(_xCoordinate, _yCoordinate, _stepPicX, _stepPicY);
            System.Drawing.Imaging.PixelFormat format = _bitmap.PixelFormat;

            Bitmap wholePicure = _bitmap.Clone(cloneRect, format);

            return wholePicure;
        }

        private static void TransformPicture(Bitmap _bitmapSource, int _xSource, int _ySource, int _width, int _height, int _xDist, int _yDist, RotateFlipType rotateFlipType)
        {
            Bitmap copyFromBitmap = WholePicture(_bitmapSource, _xSource, _ySource, _width, _height);
            using (Graphics grD = Graphics.FromImage(_bitmapSource))
            {
                GraphicsPath graphicsPath = new GraphicsPath();
                graphicsPath.AddRectangle(new Rectangle(_xDist, _yDist, _width, _height));
                grD.SetClip(graphicsPath);
                grD.Clear(Color.Transparent);
                grD.ResetClip();

                copyFromBitmap.RotateFlip(rotateFlipType);

                grD.DrawImage(copyFromBitmap, _xDist, _yDist);
            }
        }

        private static bool IsExist(List<Point> _listOfPoints, Point _point)
        {
            foreach (Point item in _listOfPoints)
            {
                if (item == _point)
                    return true;
            }

            return false;
        }

        private static int IsProportionate(Size size1, Size size2)
        {
            if (size1.Width == size2.Width && size1.Height == size2.Height)
                return 1;
            else if (size1.Height == size2.Width && size1.Width == size2.Height)
                return 2;
            return 0;
        }

        private static void ColoredAndAddCoordinate(Bitmap _bitmap, int _x, int _y, int _step, Color _color, List<Point> _list, Point _point)
        {
            ColoredDublicate(_bitmap, _x, _y, _step, _color);
            _list.Add(_point);
        }

        private static void AddPointsToHashsetList(HashSet<Point> hashSet1, HashSet<Point> hashSet2, Point point1, Point point2, List<Point> list)
        {
            hashSet1.Add(point1);
            hashSet2.Add(point2);

            list.Add(point1);
            list.Add(point2);
        }

        private static List<bool> GetWholeHash(Bitmap _bitmap, int _direction)
        {
            List<bool> listOfResults = new List<bool>();

            switch (_direction)
            {
                case 0:
                    for (int y = 0; y < _bitmap.Height; y++)
                    {
                        for (int x = 0; x < _bitmap.Width; x++)
                        {
                            if (_bitmap.GetPixel(x, y).GetBrightness() != 0)
                                listOfResults.Add(true);
                            else
                                listOfResults.Add(false);
                        }
                    }
                    break;
                case 90:
                    for (int x = _bitmap.Width - 1; x >= 0; x--)
                    {
                        for (int y = 0; y < _bitmap.Height; y++)
                        {
                            if (_bitmap.GetPixel(x, y).GetBrightness() != 0)
                                listOfResults.Add(true);
                            else
                                listOfResults.Add(false);
                        }
                    }
                    break;
                case 180:
                    for (int y = _bitmap.Height - 1; y >= 0; y--)
                    {
                        for (int x = _bitmap.Width - 1; x >= 0; x--)
                        {
                            if (_bitmap.GetPixel(x, y).GetBrightness() != 0)
                                listOfResults.Add(true);
                            else
                                listOfResults.Add(false);
                        }
                    }
                    break;
                case 270:
                    for (int x = 0; x < _bitmap.Width; x++)
                    {
                        for (int y = _bitmap.Height - 1; y >= 0; y--)
                        {
                            if (_bitmap.GetPixel(x, y).GetBrightness() != 0)
                                listOfResults.Add(true);
                            else
                                listOfResults.Add(false);
                        }
                    }
                    break;
                case 180180:
                    for (int y = 0; y < _bitmap.Height; y++)
                    {
                        for (int x = _bitmap.Width - 1; x >= 0; x--)
                        {
                            if (_bitmap.GetPixel(x, y).GetBrightness() != 0)
                                listOfResults.Add(true);
                            else
                                listOfResults.Add(false);
                        }
                    }
                    break;
                case 270270:
                    for (int y = _bitmap.Height - 1; y >= 0; y--)
                    {
                        for (int x = 0; x < _bitmap.Width; x++)
                        {
                            if (_bitmap.GetPixel(x, y).GetBrightness() != 0)
                                listOfResults.Add(true);
                            else
                                listOfResults.Add(false);
                        }
                    }
                    break;
                default:
                    break;
            }

            return listOfResults;
        }

        private static double CompareHashs(List<bool> hash1, List<bool> hash2)
        {
            int counter = 0;
            if (hash1.Count != hash2.Count)
                return 0;
            for (int i = 0; i < hash1.Count; i++)
            {
                if (hash1[i] == hash2[i])
                    counter++;
            }

            return (double)counter / hash1.Count;
        }

        private static void ColoredDublicate(Bitmap bitmap, int leftUpperCornerX, int leftUpperCornerY, int sideSizeRect, Color color)
        {
            for (int i = leftUpperCornerX + 2; i < leftUpperCornerX + sideSizeRect - 2; i++)
            {
                //Upper horizontal line
                bitmap.SetPixel(i, leftUpperCornerY + 2, color);
                bitmap.SetPixel(i, leftUpperCornerY + 3, color);

                //Right vertical line
                bitmap.SetPixel(leftUpperCornerX + sideSizeRect - 3, i + leftUpperCornerY - leftUpperCornerX, color);
                bitmap.SetPixel(leftUpperCornerX + sideSizeRect - 4, i + leftUpperCornerY - leftUpperCornerX, color);

                //Bottom horizontal line
                bitmap.SetPixel(i, leftUpperCornerY + sideSizeRect - 3, color);
                bitmap.SetPixel(i, leftUpperCornerY + sideSizeRect - 4, color);

                //Left vertical line
                bitmap.SetPixel(leftUpperCornerX + 2, i + leftUpperCornerY - leftUpperCornerX, color);
                bitmap.SetPixel(leftUpperCornerX + 3, i + leftUpperCornerY - leftUpperCornerX, color);
            }
        }

        private static void ColoredDublicateMarkCorner(Bitmap bitmap, int leftUpperCornerX, int leftUpperCornerY, Brush brush)
        {
            Point[] points = new Point[3];

            points[0].X = leftUpperCornerX;
            points[0].Y = leftUpperCornerY;

            points[1].X = leftUpperCornerX + 10;
            points[1].Y = leftUpperCornerY;

            points[2].X = leftUpperCornerX;
            points[2].Y = leftUpperCornerY + 10;

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.FillPolygon(brush, points);
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
                return new Rectangle(leftUpperCornerX, leftUpperCornerY, 1, 1);

            return new Rectangle(leftUpperCornerX, leftUpperCornerY, rightDownCornerX - leftUpperCornerX, rightDownCornerY - leftUpperCornerY);
        }

        private static List<bool> GetHash(Bitmap bitmap, int _direction, int size, float precision)
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

            return medianBrightness;
        }
    }
}
