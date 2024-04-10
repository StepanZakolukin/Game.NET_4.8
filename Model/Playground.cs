using System.IO;
using System.Linq;

namespace WindowsForm.Model
{
    public class Playground
    {
        public GameObjects[,] Map;
        public readonly int Height;
        public readonly int Width;
        public static Point[] OfSets = new Point[] { new Point(0, 1), new Point(0, -1), new Point(-1, 0), new Point(1, 0) };

        public Playground(string pathToTheFile = @"..\..\Model\WallMap.txt") 
        {
            var wallMap = File.ReadAllText(pathToTheFile).Split('\n')
                .Select(st => st.Split('\t'))
                .ToArray();
            Height = wallMap.Length - 1;
            Width = wallMap[0].Length;
            CreateMap(wallMap);
        }

        private void CreateMap(string[][] array)
        {
            Map = new GameObjects[Width, Height];

            for (var y = 0; y < Height; y++)
                for (var x = 0; x < Width; x++)
                {
                    if (array[y][x] == "0") Map[x, y] = new Stone(new Point(x, y));
                    else Map[x, y] = new Wall(new Point(x, y));
                }
        }

        public GameObjects this[int x, int y]
        {
            get { return Map[x, y]; }
            set 
            {
                Map[x, y] = value;
            }
        }

        public GameObjects this[Point point]
        {
            get { return Map[point.X, point.Y]; }
            set
            {
                Map[point.X, point.Y] = value;
            }
        }

        public bool InBounds(Point point) => point.X >= 0 && point.Y >= 0 && Width > point.X && Height > point.Y;
    }
}
