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

        public ResampleValues Rv { get; private set; }

        public FlexChain(float baud, float sampleRate, Action<MessageBase> messageReceived) : base(sampleRate, messageReceived)
        {
            this.Rv = GetResampleValues(baud, sampleRate);
            var pll = new Pll(Rv.dsr, baud);

            interpolator = new Interpolator(Rv.i);
            filter = new ChebyFilter(baud * 1.2f, 1f, Rv.isr);
            decimator = new Decimator(Rv.d);
            demodulator = new Fsk2Demodulator(baud, Rv.dsr, pll, false);
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
