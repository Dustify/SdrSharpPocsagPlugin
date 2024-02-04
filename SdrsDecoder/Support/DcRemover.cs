using System.Collections.Generic;
using System.Linq;

namespace SdrsDecoder.Support
{
    public class DcRemover
    {
        private FixedSizeQueue<float> queue;

        public DcRemover(float sampleRate, float baud, int lengthFactor = 5)
        {
            var samples_per_symbol = (int)(sampleRate / baud);
            this.queue = new FixedSizeQueue<float>(samples_per_symbol * lengthFactor);
        }

        public float Process(float value)
        {
            this.queue.Enqueue(value);

            var av = this.queue.Queue.Average();

            value -= av;

            return value;
        }

        public float[] Process(float[] values)
        {
            var result = new List<float>();

            foreach (var v in values)
            {
                result.Add(this.Process(v));
            }

            return result.ToArray();
        }
    }
}
