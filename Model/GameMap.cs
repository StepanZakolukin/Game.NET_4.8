﻿using System.IO;
using System.Linq;

namespace WindowsForm.Model
{
    public class GameMap
    {
        public GameObjects[,] Map;
        public readonly int MapHeight;
        public readonly int MapWidth;

        public GameMap(string pathToTheFile = @"..\..\Model\WallMap.txt") 
        {
            var wallMap = File.ReadAllText(pathToTheFile).Split('\n')
                .Select(st => st.Split('\t'))
                .ToArray();
            MapHeight = wallMap.Length - 1;
            MapWidth = wallMap[0].Length;
            CreateMap(wallMap);
        }

        private void CreateMap(string[][] array)
        {
            Map = new GameObjects[MapWidth, MapHeight];

            for (var y = 0; y < MapHeight; y++)
                for (var x = 0; x < MapWidth; x++)
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

        public bool InBounds(Point point) => point.X >= 0 && point.Y >= 0 && MapWidth > point.X && MapHeight > point.Y;
    }
}