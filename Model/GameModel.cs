using System.Collections.Generic;
using System.Drawing;
using System;
using System.Windows.Forms;
using MainWindow;

namespace Model
{
    public static class GameModel
    {
        public static float ImageSize;
        public static PointF InitialCoordinateOfTheMap;
        public static Soldier Player;
        public static string[][] Map;
        public static Dictionary<Point, Node> graph = new Dictionary<Point, Node>();

        public static void Start(Control control, string[][] wallMap)
        {
            Map = wallMap;
            UpdateFieldValues();
            
            for (var x = 0; x < 48; x++)
                for (var y = 0; y < 27; y++)
                    if (wallMap[y][x] == "0")
                    {
                        var point = new Point(x, y);

                        if (!graph.ContainsKey(point)) graph.Add(point, new Node(point));

                        graph[point].Forward = GetANode(point + new Point(0, 1));
                        graph[point].Left = GetANode(point + new Point(1, 0));
                        graph[point].Back = GetANode(point + new Point(0, -1));
                        graph[point].Right = GetANode(point + new Point(-1, 0));
                    }

            control.Paint += DrawAMap;
            Player = new Soldier(@"..\..\Model\Images\Red tank.png", new Point(1, 1), 90, control);
        }

        public static void UpdateFieldValues()
        {
            ImageSize = Math.Min(MyForm.Current.Size.Height / 27, MyForm.Current.Size.Width / 48);
            InitialCoordinateOfTheMap.X = (MyForm.Current.Size.Width - ImageSize * 48) / 2 - ImageSize / 3;
            InitialCoordinateOfTheMap.Y = (float)((MyForm.Current.Size.Height - ImageSize * 27) / 2 - ImageSize);
        }

        private static Node GetANode(Point neighboringPoint)
        {
            if (graph.ContainsKey(neighboringPoint)) return graph[neighboringPoint];

            if (!(neighboringPoint.X < 0 || neighboringPoint.X > 47 || neighboringPoint.Y < 0
                || neighboringPoint.Y > 26 || Map[neighboringPoint.Y][neighboringPoint.X] != "0"))
                return graph[neighboringPoint] = new Node(neighboringPoint);

            return null;
        }

        static void DrawAMap(object sender, PaintEventArgs e)
        {
            UpdateFieldValues();
            Player.UpdateTheLocation();

            var brick = Image.FromFile(@"..\..\Model\Images\кирпич.jpg");
            var stone = Image.FromFile(@"..\..\Model\Images\камень.jpg");

            for (var y = 0; y < 27; y++)
                for (var x = 0; x < 48; x++)
                {
                    var coordinatesOnTheForm = RecalculateTheCoordinatesOnTheForm(new Point(x, y));
                    if (Map[y][x] == "0") e.Graphics.DrawImage(stone, coordinatesOnTheForm);
                    else e.Graphics.DrawImage(brick, coordinatesOnTheForm);
                }
        }

        public static PointF[] RecalculateTheCoordinatesOnTheForm(Point positionOnTheMap)
        {
            return new PointF[] {
                new PointF(positionOnTheMap.X * ImageSize + InitialCoordinateOfTheMap.X, positionOnTheMap.Y * ImageSize + InitialCoordinateOfTheMap.Y),
                new PointF(positionOnTheMap.X * ImageSize + InitialCoordinateOfTheMap.X + ImageSize, positionOnTheMap.Y * ImageSize + InitialCoordinateOfTheMap.Y),
                new PointF(positionOnTheMap.X *ImageSize + InitialCoordinateOfTheMap.X, positionOnTheMap.Y * ImageSize + InitialCoordinateOfTheMap.Y + ImageSize) };
        }
    }
}
