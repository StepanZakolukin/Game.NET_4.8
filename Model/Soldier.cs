﻿using System;
using System.Drawing;
using System.Windows.Forms;
using WindowsForm;

namespace Model
{
    public class Soldier
    {
        public readonly Image Picture;
        public int AngleInDegrees { get; private set; }
        public Point PositionOnTheMap { get; private set; }
        public PaintEventHandler CurrentFunction { get; private set; }
        private readonly RotatingTheRectangle Rotator;

        public Soldier(string image, Point positionOnTheMap, int angleInDegrees, Control control)
        {
            AngleInDegrees = angleInDegrees;
            Picture = Image.FromFile(image);
            PositionOnTheMap = positionOnTheMap;
            Rotator = new RotatingTheRectangle();
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
                Picture, Rotator.RotateAnArrayOfPoints(GameModel.RecalculateTheCoordinatesOnTheForm(PositionOnTheMap), AngleInDegrees * Math.PI / 180));

        // меняет функцию отрисовки персонажа
        public void ReplaceTheCharacterRenderingFunction(Control control)
        {
            control.Paint -= CurrentFunction;
            UpdateTheLocation();
            control.Paint += CurrentFunction;
        }
    }
}
