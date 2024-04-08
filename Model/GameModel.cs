using System.Collections.Generic;
using System.Drawing;
using System;
using System.Windows.Forms;
using MainWindow;
using WindowsForm;
using WindowsForm.Model;

namespace Model
{
    public static class GameModel
    {
        private static Control FormControl;
        public static float ImageSize;
        public static PointF InitialCoordinateOfTheMap;
        public static Soldier Player;
        public static string[][] Map;
        public static Dictionary<Point, Node> GraphOfTheMapPaths = new Dictionary<Point, Node>();
        public static QueueOfActiveGameObjects<Bullet> ActivBullets = new QueueOfActiveGameObjects<Bullet>();
        public static QueueOfActiveGameObjects<Soldier> ActiveSoldiers = new QueueOfActiveGameObjects<Soldier>();

        public static void Start(Control control, string[][] wallMap)
        {
            FormControl = control;
            Map = wallMap;
            UpdateFieldValues();
            FillInThePathGraph();

            control.Paint += DrawAMap;
            Player = new Soldier(@"..\..\Model\Images\солдат.png", new Point(1, 1), 90, control);
            ActiveSoldiers.Enqueue(new Soldier(@"..\..\Model\Images\солдат.png", new Point(1, 10), 90, control));
        }

        public static void UpdateFieldValues()
        {
            ImageSize = Math.Min(MyForm.Current.Size.Height / 18 , MyForm.Current.Size.Width / 32);
            InitialCoordinateOfTheMap.X = (MyForm.Current.Size.Width - ImageSize * 32) / 2 - ImageSize / 4;
            InitialCoordinateOfTheMap.Y = (float)((MyForm.Current.Size.Height - ImageSize * 18) / 2 - ImageSize / 2);
        }

        private static Node GetANode(Point neighboringPoint)
        {
            if (GraphOfTheMapPaths.ContainsKey(neighboringPoint)) return GraphOfTheMapPaths[neighboringPoint];

            if (!(neighboringPoint.X < 0 || neighboringPoint.X > 31 || neighboringPoint.Y < 0
                || neighboringPoint.Y > 17 || Map[neighboringPoint.Y][neighboringPoint.X] != "0"))
                return GraphOfTheMapPaths[neighboringPoint] = new Node(neighboringPoint);

            return null;
        }

        static void DrawAMap(object sender, PaintEventArgs e)
        {
            UpdateFieldValues();
            Player.UpdateTheLocation();

            CheckForInactiveBullets();
            CheckForInactiveSoldiers();

            var brick = Image.FromFile(@"..\..\Model\Images\кирпич.jpg");
            var stone = Image.FromFile(@"..\..\Model\Images\камень.jpg");

            for (var y = 0; y < 18; y++)
                for (var x = 0; x < 32; x++)
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

        public static void FillInThePathGraph()
        {
            for (var x = 0; x < 32; x++)
                for (var y = 0; y < 18; y++)
                    if (Map[y][x] == "0")
                    {
                        var point = new Point(x, y);

                        if (!GraphOfTheMapPaths.ContainsKey(point)) GraphOfTheMapPaths.Add(point, new Node(point));

                        GraphOfTheMapPaths[point].Forward = GetANode(point + new Point(0, 1));
                        GraphOfTheMapPaths[point].Left = GetANode(point + new Point(1, 0));
                        GraphOfTheMapPaths[point].Back = GetANode(point + new Point(0, -1));
                        GraphOfTheMapPaths[point].Right = GetANode(point + new Point(-1, 0));
                    }
        }

        public static void CheckForInactiveSoldiers()
        {
            var countActiveSoldiers = ActiveSoldiers.Count();

            for (var i = 0; i < countActiveSoldiers; i++)
            {
                var solidier = ActiveSoldiers.Dequeue();

                if (solidier.Active) ActiveSoldiers.Enqueue(solidier);
                else FormControl.Paint -= solidier.CurrentFunction;
            }
        }

        public static void CheckForInactiveBullets()
        {
            var countActiveBillets = ActivBullets.Count();

            for (var i = 0; i < countActiveBillets; i++)
            {
                var bullet = ActivBullets.Dequeue();

                if (bullet.Active)
                {
                    bullet.MakeAnAttemptToMoveForward(FormControl);
                    ActivBullets.Enqueue(bullet);
                }
                else FormControl.Paint -= bullet.CurrentFunction;
            }
        }
    }
}
