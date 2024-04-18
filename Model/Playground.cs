using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WindowsForm.Model
{
    public class Playground
    {
        public List<GameObjects>[,] Map;
        public readonly int Height;
        public readonly int Width;

        public Playground(string pathToTheFile = @"..\..\Model\Map.txt") 
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
            Map = new List<GameObjects>[Width, Height];

            for (var y = 0; y < Height; y++)
                for (var x = 0; x < Width; x++)
                {
                    Map[x, y] = new List<GameObjects>();

                    if (array[y][x] == "0") Map[x, y].Add(new Stone(new Point(x, y)));
                    else Map[x, y].Add(new Wall(new Point(x, y)));
                }
        }

        public List<GameObjects> this[int x, int y]
        {
            get { return Map[x, y]; }
            set 
            {
                Map[x, y] = value;
            }
        }

        public List<GameObjects> this[Point point]
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
