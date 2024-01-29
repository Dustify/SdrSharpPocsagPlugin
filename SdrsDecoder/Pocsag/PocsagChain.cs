using SdrsDecoder.Support;
using System;

namespace SdrsDecoder.Pocsag
{
    public class PocsagChain : ChainBase
    {
        private float baud;
        private ChebyFilter filter;
        private Fsk2Demodulator demodulator;
        private PocsagDecoder decoder;

        public bool DISABLE_FILTER = false;

        private Interpolator interpolator;
        private Decimator decimator;


        public PocsagChain(float baud, float sampleRate, Action<MessageBase> messageReceived) : base(sampleRate, messageReceived)
        {
            this.baud = baud;

            var i = 1;
            var d = 1;

            if (baud == 1200)
            {
                i = 8;
                d = 25;
            }

            if (baud == 2400)
            {
                i = 16;
                d = 25;
            }

            var isr = sampleRate * i;
            var dsr = isr / d;

            //var pll = new PllDecimalPi(sampleRate, baud, PllUpdateType.Both, 0.2M, 0.01M);
            var pll = new Pll(dsr, baud);

            interpolator = new Interpolator(i);
            filter = new ChebyFilter(this.baud, 1f, isr);
            decimator = new Decimator(d);
            demodulator = new Fsk2Demodulator(this.baud, dsr, pll, true);
            decoder = new PocsagDecoder(Convert.ToUInt32(this.baud), messageReceived);
        }

        public override void Process(float[] values, Action<float> writeSample = null)
        {
            var filtered_values = interpolator.Process(values);

            if (!DISABLE_FILTER)
            {
                filtered_values = filter.Process(filtered_values);
            }

            var decimated_values = decimator.Process(filtered_values);

            var demodulated = demodulator.Process(decimated_values, writeSample);
            decoder.Process(demodulated);
        }
    }
}
