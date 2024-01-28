using System;
using System.Collections.Generic;

namespace SdrsDecoder.Support
{
    public enum PllUpdateType
    {
        Rising,
        Falling,
        Both
    }

    public abstract class PllDecimalBase : PllBase
    {
        private PllUpdateType type;

        protected decimal phase_per_sample;
        private decimal lo_phase;
        private bool lo_state;
        private float last_value;
        private decimal phase_error;


        public PllDecimalBase(float sampleRate, float baud, PllUpdateType type)
        {
            this.type = type;

            phase_per_sample = (decimal)baud / (decimal)sampleRate;

            if (this.type == PllUpdateType.Falling || this.type == PllUpdateType.Rising)
            {
                phase_per_sample /= 2.0M;
            }
        }

        protected abstract decimal GetAdjustment(decimal phaseError);

        public override bool Process(float value, Action<float> writeSample = null)
        {
            if (!lo_state && type == PllUpdateType.Falling && lo_phase >= 0.5M)
            {
                lo_state = true;
            }

            if (lo_state && type == PllUpdateType.Rising && lo_phase >= 0.5M)
            {
                lo_state = false;
            }

            if (lo_phase >= 1M)
            {
                // var remainder = this.lo_phase - 1M;

                // this.lo_phase = -remainder;
                lo_phase = 0M;

                switch (type)
                {
                    case PllUpdateType.Both:
                        lo_state = !lo_state;
                        break;
                    case PllUpdateType.Rising:
                        lo_state = true;
                        break;
                    case PllUpdateType.Falling:
                        lo_state = false;
                        break;
                }
            }

            var update_required = false;

            switch (type)
            {
                case PllUpdateType.Both:
                    update_required = last_value < 0f && value >= 0f || last_value >= 0f && value < 0f;
                    break;
                case PllUpdateType.Rising:
                    update_required = last_value < 0f && value >= 0f;
                    break;
                case PllUpdateType.Falling:
                    update_required = last_value >= 0f && value < 0f;
                    break;
            }

            if (update_required)
            {
                phase_error = lo_phase;

                if (phase_error > 0.5M)
                {
                    phase_error -= 1M;
                }

                var adjustment = GetAdjustment(phase_error);

                lo_phase -= adjustment;
            }

            if (writeSample != null)
            {
                writeSample(lo_state ? 1 : -1);
                writeSample((float)phase_error);
            }

            lo_phase += phase_per_sample;
            last_value = value;

            return lo_state;
        }
    }
}