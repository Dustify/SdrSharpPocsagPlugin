namespace Pocsag
{
    class PllDumb : PllBase
    {
        public PllDumb(
            float sampleRate,
            float baud,
            PllUpdateType type) : base(
            sampleRate,
            baud,
            type
            )
        {
        }

        protected override float GetAdjustment(float phaseError)
        {
            if (phaseError > 0)
            {
                return -this.phase_per_sample;
            }

            if (phaseError < 0)
            {
                return this.phase_per_sample;
            }

            return 0f;
        }
    }
}