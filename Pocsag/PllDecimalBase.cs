using System;
using System.Collections.Generic;

namespace Pocsag
{
    enum PllUpdateType
    {
        Rising,
        Falling,
        Both
    }

    abstract class PllDecimalBase
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

            this.phase_per_sample = (decimal)baud / (decimal)sampleRate;

            if (this.type == PllUpdateType.Falling || this.type == PllUpdateType.Rising)
            {
                this.phase_per_sample /= 2.0M;
            }
        }

        protected abstract decimal GetAdjustment(decimal phaseError);

        public bool Process(float value, List<float> phaseErrors = null, Action<float> writeSample = null)
        {
            if (!this.lo_state && this.type == PllUpdateType.Falling && this.lo_phase >= 0.5M)
            {
                this.lo_state = true;
            }

            if (this.lo_state && this.type == PllUpdateType.Rising && this.lo_phase >= 0.5M)
            {
                this.lo_state = false;
            }

            if (this.lo_phase >= 1M)
            {
                // var remainder = this.lo_phase - 1M;

                // this.lo_phase = -remainder;
                this.lo_phase = 0M;

                switch (this.type)
                {
                    case PllUpdateType.Both:
                        this.lo_state = !this.lo_state;
                        break;
                    case PllUpdateType.Rising:
                        this.lo_state = true;
                        break;
                    case PllUpdateType.Falling:
                        this.lo_state = false;
                        break;
                }
            }

            var update_required = false;

            switch (this.type)
            {
                case PllUpdateType.Both:
                    update_required = this.last_value < 0f && value >= 0f || this.last_value >= 0f && value < 0f;
                    break;
                case PllUpdateType.Rising:
                    update_required = this.last_value < 0f && value >= 0f;
                    break;
                case PllUpdateType.Falling:
                    update_required = this.last_value >= 0f && value < 0f;
                    break;
            }

            if (update_required)
            {
                this.phase_error = lo_phase;

                if (this.phase_error > 0.5M)
                {
                    this.phase_error -= 1M;
                }

                var adjustment = this.GetAdjustment(this.phase_error);

                this.lo_phase -= adjustment;
            }

            if (phaseErrors != null)
            {
                phaseErrors.Add((float)Math.Abs(phase_error));
            }

            if (writeSample != null)
            {
                writeSample(this.lo_state ? 1 : -1);
                writeSample((float)this.phase_error);
            }

            this.lo_phase += this.phase_per_sample;
            this.last_value = value;

            return this.lo_state;
        }
    }
}