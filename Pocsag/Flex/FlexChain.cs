using SdrsDecoder.Support;
using System;
using System.Collections.Generic;

namespace SdrsDecoder.Flex
{
    internal class FlexChain : ChainBase
    {
        private float baud;
        private ChebyFilter filter;
        private Fsk2Demodulator demodulator;
        private FlexDecoder decoder;

        public bool DISABLE_FILTER = false;

        public FlexChain(float baud, float sampleRate, Action<MessageBase> messageReceived, decimal kP = 0.2M, decimal kI = 0.01M) : base(sampleRate, messageReceived)
        {
            this.baud = baud;

            var pll = new PllDecimalPi(
              sampleRate,
              this.baud,
              PllUpdateType.Both,
              kP,
              kI,
              -10M,
              +10M
            );

            filter = new ChebyFilter(this.baud, 1f, this.sampleRate);
            demodulator = new Fsk2Demodulator(this.baud, this.sampleRate, pll, false);
            decoder = new FlexDecoder(Convert.ToUInt32(this.baud), messageReceived);
        }

        public override void Process(float[] values, List<float> phase_errors = null, Action<float> writeSample = null)
        {
            var filtered_values = values;

            if (!DISABLE_FILTER)
            {
                filtered_values = filter.Process(values);
            }

            var demodulated = demodulator.Process(filtered_values, phase_errors, writeSample);
            decoder.Process(demodulated);
        }
    }
}
