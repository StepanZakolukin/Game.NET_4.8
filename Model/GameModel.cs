using System.Collections.Generic;
using System.Windows;
using System.Drawing;
using System;
using System.Windows.Forms;
using MainWindow;

namespace Model
{
    public static class GameModel
    {
        public static Tank Player;
        public static List<Rect> LocationOfTheWalls = new List<Rect>();//
        public static string[][] Map;
        public static Dictionary<Point, Node> graph = new Dictionary<Point, Node>();

        public static void Start(Control control, string[][] wallMap)
        {
            Map = wallMap;
            Player = new Tank(@"..\..\Model\Images\Red tank.png",
                new PointF[] { new PointF(22, 588), new PointF(89, 588), new PointF(22, 634) }, -Math.PI / 2);

            for (var x = 0; x < 48; x++)
                for (var y = 0; y < 27; y++)
                {
                    if (wallMap[y][x] == "1") continue;

                    var point = new Point(x, y);

                    if (!graph.ContainsKey(point)) graph.Add(point, new Node(point));

                    graph[point].Forward = GetANode(point + new Point(0, 1));
                    graph[point].Right = GetANode(point + new Point(1, 0));
                    graph[point].Back = GetANode(point + new Point(0, -1));
                    graph[point].Forward = GetANode(point + new Point(-1, 0));
                }

            control.Paint += DrawAMap;
            control.Paint += Player.CurrentFunction;
        }

        private static Node GetANode(Point neighboringPoint)
        {
            if (graph.ContainsKey(neighboringPoint)) return graph[neighboringPoint];

            if (!(neighboringPoint.X < 0 || neighboringPoint.X > 47 || neighboringPoint.Y < 0 
                || neighboringPoint.Y > 26 || Map[neighboringPoint.Y][neighboringPoint.X] == "1"))
                return graph[neighboringPoint] = new Node(neighboringPoint);

            return null;
        }

        static void DrawAMap(object sender, PaintEventArgs e)
        {
            var sizeForm = MyForm.Current.Size;
            var size = Math.Min(sizeForm.Height / 27, sizeForm.Width / 48);
            var startX = (sizeForm.Width - size * 48) / 2 - size / 3;
            var startY = (float)((sizeForm.Height - size * 27) / 2 - size);

            var brick = Image.FromFile(@"..\..\Model\Images\кирпич.jpg");
            var stone = Image.FromFile(@"..\..\Model\Images\камень.jpg");

            for (var y = 0; y < 27; y++)
                for (var x = 0; x < 48; x++)
                {
                    if (Map[y][x] == "0") e.Graphics.DrawImage(stone,
                        new PointF[] { new PointF(startX + x * size, startY + y * size),
                        new PointF(startX + x * size + size, startY + y * size),
                        new PointF(startX + x * size, startY + y * size + size) });
                    else e.Graphics.DrawImage(brick, 
                        new PointF[] { new PointF(startX + x * size, startY + y * size),
                        new PointF(startX + x * size + size, startY + y * size),
                        new PointF(startX + x * size, startY + y * size + size) });
                }    
        }
    }
}
