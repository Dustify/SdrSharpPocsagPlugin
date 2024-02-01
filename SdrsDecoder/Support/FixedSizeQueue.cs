using System.Collections.Generic;

namespace SdrsDecoder.Support
{

    public class FixedSizeQueue<T>
    {
        public readonly List<T> Queue;
        private readonly int _limit;

        public FixedSizeQueue(int limit)
        {
            _limit = limit;
            Queue = new List<T>(limit);
        }

        public void Enqueue(T item)
        {
            if (Queue.Count >= _limit)
            {
                Queue.RemoveAt(0);
            }

            Queue.Add(item);
        }

        public int Count
        {
            get { return Queue.Count; }
        }
    }
}