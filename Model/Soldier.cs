using System;
using System.Drawing;
using System.Windows.Forms;

namespace Model
{
    public class Soldier
    {
        public readonly Image Picture;
        public int AngleInDegrees { get; private set; }
        public Point PositionOnTheMap { get; private set; }
        public PaintEventHandler CurrentFunction { get; private set; }

        public Soldier(string image, Point positionOnTheMap, int angleInDegrees, Control control)
        {
            AngleInDegrees = angleInDegrees;
            Picture = Image.FromFile(image);
            PositionOnTheMap = positionOnTheMap;
            UpdateTheLocation();
            control.Paint += CurrentFunction;
        }

        public void TurnLeft(Control control) => RotateThePlayer(control, 270);

        public void TurnRight(Control control) => RotateThePlayer(control, 90);

        public void  RotateThePlayer(Control control, int angleInDegrees)
        {
            AngleInDegrees += angleInDegrees;
            ReplaceTheCharacterRenderingFunction(control);
        }

        public void GoForward(Control control)
        {
            switch(AngleInDegrees % 360)
            {
                case 90:
                    if (GameModel.graph[PositionOnTheMap].Forward != null) PositionOnTheMap = GameModel.graph[PositionOnTheMap].Forward.Coordinates;
                    break;
                case 180:
                    if (GameModel.graph[PositionOnTheMap].Right != null) PositionOnTheMap = GameModel.graph[PositionOnTheMap].Right.Coordinates;
                    break;
                case 0:
                    if (GameModel.graph[PositionOnTheMap].Left != null) PositionOnTheMap = GameModel.graph[PositionOnTheMap].Left.Coordinates;
                    break;
                case 270:
                    if (GameModel.graph[PositionOnTheMap].Back != null) PositionOnTheMap = GameModel.graph[PositionOnTheMap].Back.Coordinates;
                    break;
            }

            ReplaceTheCharacterRenderingFunction(control);
        }

        public void GoBack(Control control)
        {
            switch (AngleInDegrees % 360)
            {
                case 90:
                    if (GameModel.graph[PositionOnTheMap].Back != null) PositionOnTheMap = GameModel.graph[PositionOnTheMap].Back.Coordinates;
                    break;
                case 180:
                    if (GameModel.graph[PositionOnTheMap].Left != null) PositionOnTheMap = GameModel.graph[PositionOnTheMap].Left.Coordinates;
                    break;
                case 0:
                    if (GameModel.graph[PositionOnTheMap].Right != null) PositionOnTheMap = GameModel.graph[PositionOnTheMap].Right.Coordinates;
                    break;
                case 270:
                    if (GameModel.graph[PositionOnTheMap].Forward != null) PositionOnTheMap = GameModel.graph[PositionOnTheMap].Forward.Coordinates;
                    break;
            }

            ReplaceTheCharacterRenderingFunction(control);
        }

        // обновляет местоположение персонажа
        public PaintEventHandler UpdateTheLocation() =>
            CurrentFunction = (sender, eventArgs) => eventArgs.Graphics.DrawImage(
                Picture, RotateAnArrayOfPoints(GameModel.RecalculateTheCoordinatesOnTheForm(PositionOnTheMap), AngleInDegrees * Math.PI / 180));

        // меняет функцию отрисовки персонажа
        public void ReplaceTheCharacterRenderingFunction(Control control)
        {
            control.Paint -= CurrentFunction;
            UpdateTheLocation();
            control.Paint += CurrentFunction;
        }
        
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
