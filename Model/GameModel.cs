﻿using System.Collections.Generic;
using System.Windows;
using System.Drawing;
using System;
using System.Windows.Forms;

namespace Model
{
    public static class GameModel
    {
        public static Tank Player;

        public static List<Rect> LocationOfTheWalls = new List<Rect>();

        public static void Start(Control control, string[][] wallMap)
        {
            Player = new Tank(@"..\..\Model\Images\Red tank.png",
                new PointF[] { new PointF(22, 588), new PointF(89, 588), new PointF(22, 634) }, -Math.PI / 2);

            for (var i = 0; i < 48; i++)
                for (var j = 0; j < 30; j++)
                {
                    if (wallMap[j][i] == "1")
                        LocationOfTheWalls.Add(new Rect(i * 22, j * 22, 22, 22));
                }

            control.Paint += DrawALawn;
            control.Paint += DrawTheWalls;
            control.Paint += Player.CurrentFunction;
        }

        static void DrawTheWalls(object sender, PaintEventArgs e)
        {
            var pictureWall = Image.FromFile(@"..\..\Model\Images\Wall.png");

            foreach (var wall in LocationOfTheWalls)
                e.Graphics.DrawImage(pictureWall, new System.Drawing.Point((int)wall.X, (int)wall.Y));
        }

        static void DrawALawn(object sender, PaintEventArgs e)
        {
            var pictureGrass = Image.FromFile(@"..\..\Model\Images\Grass.jpg");

            for (var i = 0; i < 30; i++)
                for (var j = 0; j < 48; j ++)
                    e.Graphics.DrawImage(pictureGrass, new PointF(j * 66, i * 66));
        }
    }
}