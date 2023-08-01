using System.Collections.Generic;

namespace Pocsag
{

    public class FixedSizeQueue<T>
    {
        public readonly List<T> _queue;
        private readonly int _limit;

        public FixedSizeQueue(int limit)
        {
            _limit = limit;
            _queue = new List<T>(limit);
        }

        public void Enqueue(T item)
        {
            if (_queue.Count >= _limit)
            {
                _queue.RemoveAt(0);
            }

            _queue.Add(item);
        }

        public int Count
        {
            get { return _queue.Count; }
        }
    }
}