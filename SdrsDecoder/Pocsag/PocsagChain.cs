using System;
using System.Collections.Generic;
using SdrsDecoder.Support;

namespace SdrsDecoder.Pocsag
{
    public class PocsagChain : ChainBase
    {
        private float baud;
        private ChebyFilter filter;
        private Fsk2Demodulator demodulator;
        private PocsagDecoder decoder;

        public bool DISABLE_FILTER = false;

        public PocsagChain(float baud, float sampleRate, Action<MessageBase> messageReceived) : base(sampleRate, messageReceived)
        {
            this.baud = baud;
            
            //var pll = new PllDecimalPi(sampleRate, baud, PllUpdateType.Both, 0.2M, 0.01M);
            var pll = new Pll(sampleRate, baud);

            filter = new ChebyFilter(this.baud, 1f, sampleRate);
            demodulator = new Fsk2Demodulator(this.baud, sampleRate, pll, true);
            decoder = new PocsagDecoder(Convert.ToUInt32(this.baud), messageReceived);
        }

        public override void Process(float[] values, Action<float> writeSample = null)
        {
            var filtered_values = values;

            if (!DISABLE_FILTER)
            {
                filtered_values = filter.Process(filtered_values);
            }

            var demodulated = demodulator.Process(filtered_values, writeSample);
            decoder.Process(demodulated);
        }
    }
}
