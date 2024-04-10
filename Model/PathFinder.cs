using System.Collections.Generic;

namespace WindowsForm.Model
{
    public class PathFinder
    {
        public static SinglyLinkedList<Point> FindPaths(Point start, Point purpose)
        {
            var queue = new Queue<SinglyLinkedList<Point>>();
            queue.Enqueue(new SinglyLinkedList<Point>(start));
            var visited = new HashSet<Point>() { start };

            while (queue.Count > 0)
            {
                var point = queue.Dequeue();

                if (!GameModel.Map.InBounds(point.Value) || GameModel.Map[point.Value] is Wall
                    || GameModel.Map[point.Value] is Soldier || GameModel.Map[point.Value] is Bullet)
                    continue;

                if (point.Value == purpose) return point;

                foreach (var offset in GameModel.OfSets)
                {
                    var nextPoint = point.Value + offset;

                    if (visited.Contains(nextPoint)) continue;

                    visited.Add(nextPoint);
                    queue.Enqueue(new SinglyLinkedList<Point>(nextPoint, point));
                }
            }

            return null;
        }
    }
}
