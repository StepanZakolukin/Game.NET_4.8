using MainWindow;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Model
{
    public class Tank
    {
        readonly double StepLength;
        public readonly Image Picture;
        readonly double AngularDisplacement;
        public double Angle { get; private set; }
        public PointF[] Position { get; set; }
        public PaintEventHandler CurrentFunction { get; private set; }

        public Tank(string image, PointF[] position, double angle)
        {
            AngularDisplacement = 0.1;
            Angle = angle;
            Position = position;
            StepLength = 10;
            Picture = Image.FromFile(image);
            UpdateTheLocation();
        }

        public void TurnLeft(Control control) => 
            CheckTheCorrectnessOfTheMovement(Position, -AngularDisplacement, control);
            
        public void TurnRight(Control control) => 
            CheckTheCorrectnessOfTheMovement(Position, AngularDisplacement, control);

        public void GoForward(Control control)
        {
            CheckTheCorrectnessOfTheMovement(Position
                .Select(point => new PointF((float)(point.X + StepLength * Math.Cos(Angle)), (float)(point.Y + StepLength * Math.Sin(Angle))))
                .ToArray(), 0, control);
        }

        public void GoBack(Control control)
        {
            CheckTheCorrectnessOfTheMovement(Position
                .Select(point => new PointF((float)(point.X - StepLength * Math.Cos(Angle)), (float)(point.Y - StepLength * Math.Sin(Angle))))
                .ToArray(), 0, control);
        }

        // проверка коректности движения
        private void CheckTheCorrectnessOfTheMovement(PointF[] coordinates, double angle, Control control)
        {
            var sortedСoordinates = coordinates.OrderBy(point => point.X + point.Y).ToArray();

            var tankOutline = new System.Windows.Rect(sortedСoordinates[0].X, sortedСoordinates[0].Y,
                Math.Sqrt((sortedСoordinates[1].X - sortedСoordinates[0].X) * (coordinates[1].X - sortedСoordinates[0].X) + 
                (sortedСoordinates[1].Y - sortedСoordinates[0].Y) * (sortedСoordinates[1].Y - sortedСoordinates[0].Y)), 
                Math.Sqrt((sortedСoordinates[2].X - sortedСoordinates[0].X) * (sortedСoordinates[2].X - sortedСoordinates[0].X) +
                (sortedСoordinates[2].Y - sortedСoordinates[0].Y) * (sortedСoordinates[2].Y - sortedСoordinates[0].Y)));

            if (coordinates.All(point => point.X >= 0 && point.Y >= 0 && point.X <= MyForm.FieldDimensions.Width && point.Y <= MyForm.FieldDimensions.Height) 
                && !GameModel.LocationOfTheWalls.Any(outlineOfTheWall => Intersection(tankOutline, outlineOfTheWall)))
            {
                control.Paint -= CurrentFunction;
                Position = coordinates;
                Angle += angle;
                UpdateTheLocation();
                control.Paint += CurrentFunction;
            }
        }

        bool Intersection(System.Windows.Rect rect1, System.Windows.Rect rect2)
        {
            var centre1 = new PointF((float)(rect1.X + rect1.Width / 2), (float)(rect1.Y + rect1.Height / 2));
            var centre2 = new PointF((float)(rect2.X + rect2.Width / 2), (float)(rect2.Y + rect2.Height / 2));

            return Math.Sqrt((centre2.Y - centre1.Y) * (centre2.Y - centre1.Y) + (centre2.X - centre1.X) * (centre2.X - centre1.X)) -
                Math.Sqrt((rect1.Height / 2) * (rect1.Height / 2) + (rect1.Width / 2) * (rect1.Width / 2)) + 
                Math.Sqrt((rect2.Height / 2) * (rect2.Height / 2) + (rect2.Width / 2) * (rect2.Width / 2)) <= 0f;
        }

        // обновить местоположение
        private PaintEventHandler UpdateTheLocation() =>
            CurrentFunction = (sender, eventArgs) => eventArgs.Graphics.DrawImage(Picture, RotateAnArrayOfPoints(Position, Angle));

        private PointF[] RotateAnArrayOfPoints(PointF[] points, double turn)
        {
            var centre = new PointF(points[0].X + (points[1].X - points[0].X) / 2, points[0].Y + (points[2].Y - points[0].Y) / 2);

            var point1 = RotateAPoint(new PointF(points[0].X - centre.X, points[0].Y - centre.Y), turn);
            var point2 = RotateAPoint(new PointF(points[1].X - centre.X, points[1].Y - centre.Y), turn);
            var point3 = RotateAPoint(new PointF(points[2].X - centre.X, points[2].Y - centre.Y), turn);

            return new PointF[]
            {
                new PointF(centre.X + point1.X,centre.Y + point1.Y),
                new PointF(centre.X + point2.X, centre.Y + point2.Y),
                new PointF(centre.X + point3.X, centre.Y + point3.Y)
            };
        }

        private PointF RotateAPoint(PointF point, double angleInRadians)
        {
            var d = Math.Sqrt(point.X * point.X + point.Y * point.Y);
            var angle = Math.Atan2(point.Y, point.X) + angleInRadians;

            return new PointF((float)(Math.Cos(angle) * d), (float)(Math.Sin(angle) * d));
        }
    }
}