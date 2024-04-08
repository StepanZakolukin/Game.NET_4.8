using Model;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsForm.Model
{
    public class MobileGameObject
    {
        public bool Active;
        public int Priority;
        public readonly Image Picture;
        public int AngleInDegrees { get; set; }
        public Point PositionOnTheMap { get; set; }
        public PaintEventHandler CurrentFunction { get; set; }

        public MobileGameObject(string image, Point positionOnTheMap, int angleInDegrees, Control control)
        {
            AngleInDegrees = angleInDegrees;
            Picture = Image.FromFile(image);
            PositionOnTheMap = positionOnTheMap;
            Active = true;
            UpdateTheLocation();
            control.Paint += CurrentFunction;
        }

        public bool MakeAnAttemptToMoveForward(Control control)
        {
            var result = false;

            switch (AngleInDegrees % 360)
            {
                case 90:
                    result = Move(GameModel.GraphOfTheMapPaths[PositionOnTheMap].Forward);
                    ReplaceTheCharacterRenderingFunction(control);
                    return result;
                case 180:
                    result = Move(GameModel.GraphOfTheMapPaths[PositionOnTheMap].Right);
                    ReplaceTheCharacterRenderingFunction(control);
                    return result;
                case 0:
                    result = Move(GameModel.GraphOfTheMapPaths[PositionOnTheMap].Left);
                    ReplaceTheCharacterRenderingFunction(control);
                    return result;
                case 270:
                    result = Move(GameModel.GraphOfTheMapPaths[PositionOnTheMap].Back);
                    ReplaceTheCharacterRenderingFunction(control);
                    return result;
            }

            return result;
        }

        public bool Move(Node NextNode)
        {
            if (NextNode != null)
            {
                if (!GameModel.ActiveSoldiers.Сontains(NextNode.Coordinates))
                {
                    PositionOnTheMap = NextNode.Coordinates;
                    return true;
                }
                else
                    if (100 != Priority)
                    {
                        GameModel.ActiveSoldiers[NextNode.Coordinates].Active = false;
                        Active = false;
                    }
            }

            Active = false;
            return false;
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

        public PointF[] RotateAnArrayOfPoints(PointF[] points, double turn)
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
