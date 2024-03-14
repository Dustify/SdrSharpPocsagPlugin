using System;
using System.Collections.Generic;

namespace SdrsDecoder.Support
{
    public enum PllMode
    {
        Both,
        Rising
    }

    public class Pll : PllBase
    {
        protected int phase_per_sample;
        private int lo_phase;
        private bool lo_state;
        private float last_value;
        private int phase_error;
        private int half_phase_per_sample;
        private PllMode mode;

        public Pll(float sampleRate, float baud, PllMode mode = PllMode.Both)
        {
            phase_per_sample = (int)sampleRate / (int)baud;
            half_phase_per_sample = phase_per_sample / 2;
            this.mode = mode;
        }

        public override bool Process(float value, Action<float> writeSample = null)
        {
            if (lo_phase >= phase_per_sample)
            {
                lo_phase -= phase_per_sample;
                lo_state = !lo_state;
            }

            var valueStateChanged = false;

            if (mode == PllMode.Both)
            {
                valueStateChanged = last_value >= 0 != value >= 0;
            }

            if (mode == PllMode.Rising)
            {
                valueStateChanged = last_value < 0 && value >= 0;
            }

            if (valueStateChanged)
            {
                phase_error = lo_phase;

                if (phase_error != 0)
                {
                    if (phase_error > half_phase_per_sample)
                    {
                        phase_error -= phase_per_sample;
                    }

                    if (phase_error > 0)
                    {
                        lo_phase--;
                    }
                    else
                    {
                        lo_phase++;
                    }
                }
            }

            if (writeSample != null)
            {
                writeSample(value);
                writeSample(lo_state ? 1 : -1);
                writeSample((float)phase_error / (float)phase_per_sample);
            }

            lo_phase++;
            last_value = value;

            return lo_state;
        }

        public bool[] Process(float[] values, Action<float> writeSample = null)
        {
            var result = new List<bool>();

            foreach (var v in values)
            {
                result.Add(Process(v, writeSample));
            }

            return result.ToArray();
        }
    }
}