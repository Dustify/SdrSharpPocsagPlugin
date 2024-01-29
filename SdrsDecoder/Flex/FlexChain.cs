using SdrsDecoder.Support;
using System;

namespace SdrsDecoder.Flex
{
    public class FlexChain : ChainBase
    {
        private ChebyFilter filter;
        private Fsk2Demodulator demodulator;
        private FlexDecoder decoder;
        private Interpolator interpolator;
        private Decimator decimator;

        public FlexChain(float baud, float sampleRate, Action<MessageBase> messageReceived) : base(sampleRate, messageReceived)
        {
            var rv = GetResampleValues(baud, sampleRate);
            var pll = new Pll(rv.dsr, baud);

            interpolator = new Interpolator(rv.i);
            filter = new ChebyFilter(baud, 1f, rv.isr);
            decimator = new Decimator(rv.d);
            demodulator = new Fsk2Demodulator(baud, rv.dsr, pll, false);
            decoder = new FlexDecoder(Convert.ToUInt32(baud), messageReceived);
        }

        public override void Process(float[] values, Action<float> writeSample = null)
        {
            var processed_values = interpolator.Process(values);
            processed_values = filter.Process(processed_values);
            processed_values = decimator.Process(processed_values);

            var demodulated_values = demodulator.Process(processed_values, writeSample);
            decoder.Process(demodulated_values);
        }
    }
}
