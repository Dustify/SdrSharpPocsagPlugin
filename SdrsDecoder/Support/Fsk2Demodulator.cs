using System;
using System.Collections.Generic;
using System.Linq;

namespace SdrsDecoder.Support
{
    public class Fsk2Demodulator
    {
        private FixedSizeQueue<float> value_fifo;
        private bool invert;
        private PllBase pll;
        private bool last_lo_state;
        private bool output_state;

        public Fsk2Demodulator(float baud, float sampleRate, PllBase pll, bool invert)
        {
            this.pll = pll;

            var samples_per_symbol = (int)Math.Round(sampleRate / baud);

            value_fifo = new FixedSizeQueue<float>(samples_per_symbol);
            this.invert = invert;
        }

        public bool[] Process(float[] values, Action<float> writeSample = null)
        {
            var result = new List<bool>();

            for (var i = 0; i < values.Length; i++)
            {
                var value = values[i];
                value_fifo.Enqueue(value);

                var lo_state = pll.Process(value, writeSample);

                if (lo_state != last_lo_state)
                {
                    output_state = value_fifo.Queue.Average() >= 0;
                    result.Add(invert ? !output_state : output_state);
                }

                if (writeSample != null)
                {
                    writeSample(output_state ? 1f : -1f);
                }

                last_lo_state = lo_state;
            }

            return result.ToArray();
        }
    }
}
