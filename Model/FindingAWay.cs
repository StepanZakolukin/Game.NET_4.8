﻿using System.Collections.Generic;

namespace WindowsForm.Model
{
    public static class FindingAWay
    {
        public static IEnumerable<SinglyLinkedList<Point>> FindAWay(Playground map, Point finish, HashSet<Point> startingPositions)
        {
            var queue = new Queue<SinglyLinkedList<Point>>();
            queue.Enqueue(new SinglyLinkedList<Point>(finish));
            var visited = new HashSet<Point>() { finish };

            while (queue.Count > 0)
            {
                var point = queue.Dequeue();

                if (!map.InBounds(point.Value) || map[point.Value] is Wall
                    || map[point.Value] is Bullet || map[point.Value] is Bot)
                    continue;

                if (startingPositions.Contains(point.Value)) yield return point;

                foreach (var ofset in Walker.OfSets)
                {
                    var nextPoint = point.Value + ofset;

                    if (visited.Contains(nextPoint)) continue;

                    visited.Add(nextPoint);
                    queue.Enqueue(new SinglyLinkedList<Point>(nextPoint, point));
                }
            }

            yield break;
        }
    }
}
