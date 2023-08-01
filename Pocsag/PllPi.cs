namespace Pocsag
{
    class PllPi : PllBase
    {
        private float kp;
        private float ki;
        private float ki_min;
        private float ki_max;
        private float integration_error;

        public PllPi(
            float sampleRate,
            float baud,
            PllUpdateType type,
            float kP,
            float kI,
            float kIMin = float.MinValue,
            float kIMax = float.MaxValue) : base(
            sampleRate,
            baud,
            type
            )
        {
            this.kp = kP;
            this.ki = kI;
            this.ki_min = kIMin;
            this.ki_max = kIMax;
        }

        protected override float GetAdjustment(float phaseError)
        {
            this.integration_error += phaseError;

            if (this.integration_error > this.ki_max)
            {
                this.integration_error = this.ki_max;
            }
            else if (this.integration_error < this.ki_min)
            {
                this.integration_error = this.ki_min;
            }

            var adjustment = (phaseError * this.kp) + (this.integration_error * this.ki);

            return adjustment;
        }
    }
}