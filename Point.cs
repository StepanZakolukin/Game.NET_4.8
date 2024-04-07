﻿ public struct Point
{
    public Point(int x, int y)
    {
        X = x; Y = y;
    }
    public int X { get; set; }
    public int Y { get; set; }

    public static Point operator +(Point point1, Point point2) => new Point(point1.X + point2.X, point1.Y + point2.Y);
}
