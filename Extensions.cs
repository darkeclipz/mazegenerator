using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeApp
{
    static class BitmapExtensions
    {
        public static void DrawRaster(this Bitmap bitmap, int thickness, int padding)
        {
            for (int i = 0; i < bitmap.Width; i += padding + thickness)
                for (int j = 0; j < bitmap.Height; j++)
                {
                    bitmap.SetPixel(i, j, Color.DarkGray);
                    bitmap.SetPixel(j, i, Color.DarkGray); 
                }
        }
    }
}
