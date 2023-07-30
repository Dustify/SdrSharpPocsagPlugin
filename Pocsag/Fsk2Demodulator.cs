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
        private int samples_per_symbol;
        private int samples_per_symbol_half;
        private float error_per_symbol;
        private int lo_phase;
        private bool lo_state;
        private int zc_phase;

        private FixedSizeQueue<float> fifo;

        private float last_value;
        private bool output_state;
        private float dec_error;

        public Fsk2Demodulator(float baud, float sampleRate)
        {
            this.baud = baud;

            var samples_per_symbol_real = (float)sampleRate / baud;

            this.samples_per_symbol = (int)Math.Round(samples_per_symbol_real);
            this.samples_per_symbol_half = (int)Math.Round((float)samples_per_symbol / 2f);

            this.error_per_symbol = samples_per_symbol_real - this.samples_per_symbol;


            this.fifo = new FixedSizeQueue<float>(samples_per_symbol);
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
                        if (lo_phase >= samples_per_symbol_half)
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

                if (lo_phase >= samples_per_symbol)
                {
                    lo_phase = 0;
                    lo_state = !lo_state;

                    output_state = fifo._queue.Average() < 0;
                    result.Add(output_state);

                    dec_error += error_per_symbol;

                    if (this.dec_error >= 1f)
                    {
                        this.lo_phase--;
                        this.dec_error -= 1f;
                    }
                    else if (this.dec_error <= 1f)
                    {
                        this.lo_phase++;
                        this.dec_error += 1f;
                    }
                }
            }

            return result.ToArray();
        }
    }
}
