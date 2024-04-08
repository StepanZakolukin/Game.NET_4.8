using System.Collections.Generic;
using WindowsForm.Model;

namespace WindowsForm
{
    public class QueueOfActiveGameObjects<T>
        where T : MobileGameObject
    {
        private readonly Queue<T> Queue = new Queue<T>();
        private readonly Dictionary<Point, T> Dict = new Dictionary<Point, T>();

        public void Enqueue(T item)
        {
            Queue.Enqueue(item);
            Dict[item.PositionOnTheMap] = item;
        }

        public T Dequeue()
        {
            var result = Queue.Dequeue();
            Dict.Remove(result.PositionOnTheMap);
            return result;
        }

        public bool Сontains(Point point) => Dict.ContainsKey(point);
        public int Count() => Dict.Count;

        public T this[Point point] => Dict[point];
    }
}
