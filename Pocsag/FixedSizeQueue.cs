using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pocsag
{
    internal class FixedSizeQueue<T>
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
