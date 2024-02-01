using SdrsDecoder.Support;
using System;

namespace SdrsDecoder.Ax25
{
    public class Ax25Chain : ChainBase
    {
        private Interpolator interpolator;
        private ChebyFilter filter;
        private Decimator decimator;
        private IqDemod iqDemodulator;
        private Fsk2Demodulator fskDemodulator;
        private NrzDecoder nrzDecoder;
        private Ax25Decoder ax25Decoder;

        public ResampleValues Rv { get; private set; }

        public Ax25Chain(float sampleRate, Action<MessageBase> messageReceived) : base(sampleRate, messageReceived)
        {
            var baud = 1200;
            var mark = 1200;
            var space = 2200;

            this.Rv = GetResampleValues(baud, sampleRate);
            var pll = new Pll(Rv.dsr, baud);

            interpolator = new Interpolator(Rv.i);
            filter = new ChebyFilter(space, 1f, Rv.isr);
            decimator = new Decimator(Rv.d);
            iqDemodulator = new IqDemod(Rv.dsr, baud, mark, space);
            fskDemodulator = new Fsk2Demodulator(baud, Rv.dsr, pll, false);
            nrzDecoder = new NrzDecoder();
            ax25Decoder = new Ax25Decoder(messageReceived);
        }

        public override void Process(float[] values, Action<float> writeSample = null)
        {
            var processed_values = interpolator.Process(values);
            processed_values = filter.Process(processed_values);
            processed_values = decimator.Process(processed_values);
            processed_values = iqDemodulator.Process(processed_values);

            var fsk_demodulated_values = fskDemodulator.Process(processed_values, writeSample);
            var nrz_decoded_values = nrzDecoder.Process(fsk_demodulated_values);

            ax25Decoder.Process(nrz_decoded_values);
        }
    }
}
