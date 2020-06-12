using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public static class Extensions
    {

        public static Bitmap Crop(Bitmap _bitmap, Rectangle _rectangle)
        {
            return _bitmap.Clone(_rectangle, _bitmap.PixelFormat);
        }
    }
}
