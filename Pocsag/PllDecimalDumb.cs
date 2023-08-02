namespace Pocsag
{
    class PllDecimalDumb : PllDecimalBase
    {
        public PllDecimalDumb(
            float sampleRate,
            float baud,
            PllUpdateType type) : base(
            sampleRate,
            baud,
            type
            )
        {
        }

        protected override decimal GetAdjustment(decimal phaseError)
        {
            if (phaseError > 0)
            {
                return +this.phase_per_sample;
            }

            if (phaseError < 0)
            {
                return -this.phase_per_sample;
            }

            return 0M;
        }
    }
}