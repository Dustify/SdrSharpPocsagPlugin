namespace Pocsag
{
    class PllDecimalPi : PllDecimalBase
    {
        private decimal kp;
        private decimal ki;
        private decimal ki_min;
        private decimal ki_max;
        private decimal integration_error;

        public PllDecimalPi(
            float sampleRate,
            float baud,
            PllUpdateType type,
            decimal kP,
            decimal kI,
            decimal kIMin = decimal.MinValue,
            decimal kIMax = decimal.MaxValue) : base(
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

        protected override decimal GetAdjustment(decimal phaseError)
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