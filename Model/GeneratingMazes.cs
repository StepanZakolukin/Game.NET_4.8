using System;

namespace WindowsForm.Model
{
    public static class GeneratingMazes
    {
        public static string[,] GenerateAMaze(int width, int height)
        {
            var matrix = new string[width, height];

            for (var x = 0; x < width; x++)
                for (var y = 0; y < height; y++)
                {
                    if (x == 0 || x == width - 1 || y == height - 1 || y == 0) matrix[x, y] = "1";
                    else matrix[x, y] = "0";
                }

            CreatingWalls(matrix, 0, 1, width - 2, 1, height - 2);

            return matrix;
        }

        static void CreatingWalls(string[,] matrix, int nestingLevel, int startX, int endX, int startY, int endY)
        {
            if (endX - startX < 4 && endY - startY < 4) return;

            if (nestingLevel % 2 == 0)
            {
                var x = startX + (endX - startX + 1) / 2;
                var YCoordinateOfThePassage = new Random().Next(startY, endY + 1);

                for (var y = startY; y <= endY; y++)
                    matrix[x, y] = "1";

                while (x % 4 == 0 && YCoordinateOfThePassage % 4 == 0)
                    YCoordinateOfThePassage = new Random().Next(startY, endY + 1);

                matrix[x, YCoordinateOfThePassage] = "0";

                CreatingWalls(matrix, nestingLevel + 1, startX, x - 1, startY, endY);
                CreatingWalls(matrix, nestingLevel + 1, x + 1, endX, startY, endY);
            }
            else
            {
                var y = startY + (endY - startY + 1) / 2;
                var XCoordinateOfThePassage = new Random().Next(startX, endX + 1);

                for (var x = startX; x <= endX; x++)
                    matrix[x, y] = "1";

                while (y % 4 == 0 && XCoordinateOfThePassage % 4 == 0)
                    XCoordinateOfThePassage = new Random().Next(startX, endX + 1);

                matrix[XCoordinateOfThePassage, y] = "0";

                CreatingWalls(matrix, nestingLevel + 1, startX, endX, startY, y - 1);
                CreatingWalls(matrix, nestingLevel + 1, startX, endX, y + 1, endY);
            }
        }
    }
}
