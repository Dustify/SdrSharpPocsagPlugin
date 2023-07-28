using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pocsag
{
    internal class Fsk2Demodulator
    {
        private float baud;
        private int samples_per_bit;
        private int samples_per_bit_half;

        private int lo_phase;
        private bool lo_state;
        private int zc_phase;

        private FixedSizeQueue<float> fifo;

        private float last_value;
        private bool output_state;

        public Fsk2Demodulator(float baud, float sampleRate)
        {
            this.baud = baud;
            this.samples_per_bit = (int)Math.Round((float)sampleRate / baud);
            this.samples_per_bit_half = (int)Math.Round((float)samples_per_bit / 2f);

            this.fifo = new FixedSizeQueue<float>(samples_per_bit);
        }

        public bool[] Process(float[] values)
        {
            var result = new List<bool>();

            foreach (var value in values)
            {
                fifo.Enqueue(value);

                // zero cross
                if (value >= 0 && last_value < 0 || value < 0 && last_value >= 0)
                {
                    zc_phase = 0;

                    if (lo_phase != 0)
                    {
                        if (lo_phase >= samples_per_bit_half)
                        {
                            lo_phase++;
                        }
                        else
                        {
                            lo_phase--;
                        }
                    }
                }

                last_value = value;

                lo_phase++;
                zc_phase++;

                if (lo_phase >= samples_per_bit)
                {
                    lo_phase = 0;
                    lo_state = !lo_state;

                    output_state = fifo._queue.Average() < 0;
                    result.Add(output_state);
                }
            }

            return result.ToArray();
        }
    }
}
