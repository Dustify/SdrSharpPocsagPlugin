namespace SdrsDecoder.Support.mathnet
{
    public interface IOnlineFilter
    {
        /// <summary>
        /// Process a single sample.
        /// </summary>
        float ProcessSample(float sample);

        /// <summary>
        /// Process a whole set of samples at once.
        /// </summary>
        float[] ProcessSamples(float[] samples);

        /// <summary>
        /// Reset internal state (not coefficients!).
        /// </summary>
        void Reset();
    }
}
