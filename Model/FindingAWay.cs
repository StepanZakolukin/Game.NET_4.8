using System.Collections.Generic;

namespace WindowsForm.Model
{
    public static class FindingAWay
    {
        public static IEnumerable<SinglyLinkedList<Point>> FindAWay(Point finish, HashSet<Point> startingPositions)
        {
            var queue = new Queue<SinglyLinkedList<Point>>();
            queue.Enqueue(new SinglyLinkedList<Point>(finish));
            var visited = new HashSet<Point>() { finish };

            while (queue.Count > 0)
            {
                var point = queue.Dequeue();

                if (!GameModel.Map.InBounds(point.Value) || GameModel.Map[point.Value] is Wall
                    || GameModel.Map[point.Value] is Bullet || GameModel.Map[point.Value] is Bot)
                    continue;

                if (startingPositions.Contains(point.Value)) yield return point;

                foreach (var offset in GameModel.OfSets)
                {
                    var nextPoint = point.Value + offset;

                    if (visited.Contains(nextPoint)) continue;

                    visited.Add(nextPoint);
                    queue.Enqueue(new SinglyLinkedList<Point>(nextPoint, point));
                }
            }

            yield break;
        }
    }
}
