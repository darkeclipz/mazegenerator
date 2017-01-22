using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeApp
{
    // :-)
    internal class EdgeDetection
    {
        private int[,] kernelHorizontal_3x3 = new int[3, 3] { { -1, -1, -1 },
                                                              {  1,  1,  1 },
                                                              { -1, -1, -1 } };

        private int[,] kernelVertical_3x3 = new int[3, 3] {   { -1,  1, -1 },
                                                              { -1,  1, -1 },
                                                              { -1,  1, -1 } };

        public Bitmap Detect(Bitmap bitmap)
        {
            var originalMaze = bitmap;
            var grayscaledMaze = GetGrayscaleInverted(originalMaze);
            var edgedMaze = new Bitmap(originalMaze.Width, originalMaze.Height);

            for (int i = 0; i < originalMaze.Width; i++)
            {
                for (int j = 0; j < originalMaze.Height; j++)
                {
                    // Fill a matrix with the current selected pixel. Keep track of the edges so those values become 0.
                    // TODO: Also, this thing is an absolute mess, rather just assign the 9 pixels individually.
                    int[,] matrix = new int[3, 3] {  { i-1 > 0 && j-1 > 0                   ? grayscaledMaze[i-1,j-1] : 0, j-1 > 0                   ? grayscaledMaze[i,j-1] : 0, i+1 < originalMaze.Height && j-1 > 0                  ? grayscaledMaze[i+1,j-1] : 0 },
                                                     { i-1 > 0                              ? grayscaledMaze[i-1,j]   : 0,                             grayscaledMaze[i,j],       i+1 < originalMaze.Width                              ? grayscaledMaze[i+1,j]   : 0 },
                                                     { i-1 > 0 && j+1 < originalMaze.Height ? grayscaledMaze[i-1,j+1] : 0, j+1 < originalMaze.Height ? grayscaledMaze[i,j+1] : 0, i+1 < originalMaze.Width && j+1 < originalMaze.Height ? grayscaledMaze[i+1,j+1] : 0 } };

                    // Multiply edge with kernel
                    bool hEdge =  (matrix[0, 0] * kernelHorizontal_3x3[0, 0]) + (matrix[0, 1] * kernelHorizontal_3x3[0, 1]) + (matrix[0, 2] * kernelHorizontal_3x3[0, 2])
                                + (matrix[1, 0] * kernelHorizontal_3x3[1, 0]) + (matrix[1, 1] * kernelHorizontal_3x3[1, 1]) + (matrix[1, 2] * kernelHorizontal_3x3[1, 2])
                                + (matrix[2, 0] * kernelHorizontal_3x3[2, 0]) + (matrix[2, 1] * kernelHorizontal_3x3[2, 1]) + (matrix[2, 2] * kernelHorizontal_3x3[2, 2]) > 0;

                    // Multiply edge with kernel
                    bool vEdge =  (matrix[0, 0] * kernelVertical_3x3[0, 0]) + (matrix[0, 1] * kernelVertical_3x3[0, 1]) + (matrix[0, 2] * kernelVertical_3x3[0, 2])
                                + (matrix[1, 0] * kernelVertical_3x3[1, 0]) + (matrix[1, 1] * kernelVertical_3x3[1, 1]) + (matrix[1, 2] * kernelVertical_3x3[1, 2])
                                + (matrix[2, 0] * kernelVertical_3x3[2, 0]) + (matrix[2, 1] * kernelVertical_3x3[2, 1]) + (matrix[2, 2] * kernelVertical_3x3[2, 2]) > 0;

                    // We found an edge
                    if (hEdge || vEdge)
                        edgedMaze.SetPixel(i, j, Color.Red);
                }
            }

            return edgedMaze;
        }

        private int[,] GetGrayscaleInverted(Image image)
        {
            var grayscaledImage = new int[image.Width, image.Height];
            Bitmap bitmap = new Bitmap(image);

            for (int i = 0; i < image.Width; i++)
                for (int j = 0; j < image.Height; j++)
                {
                    var pixel = bitmap.GetPixel(i, j);

                    // Luminosity conversion, and invert.
                    grayscaledImage[i, j] = 254 - (int)(0.21 * pixel.R + 0.72 * pixel.G + 0.07 * pixel.B);
                }

            return grayscaledImage;
        }
    }
}
