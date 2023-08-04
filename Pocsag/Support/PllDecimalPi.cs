namespace Pocsag.Support
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
            kp = kP;
            ki = kI;
            ki_min = kIMin;
            ki_max = kIMax;
        }

        protected override decimal GetAdjustment(decimal phaseError)
        {
            integration_error += phaseError;

            if (integration_error > ki_max)
            {
                integration_error = ki_max;
            }
            else if (integration_error < ki_min)
            {
                integration_error = ki_min;
            }

            var adjustment = phaseError * kp + integration_error * ki;

            return adjustment;
        }
    }
}