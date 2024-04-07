using Model;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsForm.Model
{
    public class Bullet
    {
        public readonly Image Picture;
        public int AngleInDegrees { get; private set; }
        public Point PositionOnTheMap { get; private set; }
        public PaintEventHandler CurrentFunction { get; private set; }
        private readonly RotatingTheRectangle Rotator;
        public Bullet(string image, Point positionOnTheMap, int angleInDegrees, Control control)
        {
            AngleInDegrees = angleInDegrees;
            Picture = Image.FromFile(image);
            PositionOnTheMap = positionOnTheMap;
            Rotator = new RotatingTheRectangle();
            UpdateTheLocation();
            control.Paint += CurrentFunction;
        }

        public void GoForward(Control control)
        {
            switch (AngleInDegrees % 360)
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

        // обновляет местоположение пули
        public PaintEventHandler UpdateTheLocation() =>
            CurrentFunction = (sender, eventArgs) => eventArgs.Graphics.DrawImage(
                Picture, Rotator.RotateAnArrayOfPoints(GameModel.RecalculateTheCoordinatesOnTheForm(PositionOnTheMap), AngleInDegrees * Math.PI / 180));

        // меняет функцию отрисовки пули
        public void ReplaceTheCharacterRenderingFunction(Control control)
        {
            control.Paint -= CurrentFunction;
            UpdateTheLocation();
            control.Paint += CurrentFunction;
        }
    }
}
