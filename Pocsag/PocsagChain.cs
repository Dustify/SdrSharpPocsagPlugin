using System;
using System.Collections.Generic;

namespace Pocsag
{
    internal class PocsagChain : ChainBase
    {
        private float bps;
        private ChebyFilter filter;
        private Fsk2Demodulator demodulator;
        private PocsagDecoder decoder;

        public PocsagChain(float bps, float sampleRate, Action<PocsagMessage> messageReceived) : base(sampleRate, messageReceived)
        {
            this.bps = bps;

            // var pll = new PllPi(
            //     sampleRate,
            //     this.bps,
            //     PllUpdateType.Both,
            //     kP,
            //     kI
            // );


            var pll = new PllDumb(
                sampleRate,
                this.bps,
                PllUpdateType.Both
            );

            this.filter = new ChebyFilter(this.bps * 2f, 1f, this.sampleRate);
            this.demodulator = new Fsk2Demodulator(this.bps, this.sampleRate, pll, true);
            this.decoder = new PocsagDecoder(Convert.ToUInt32(this.bps), messageReceived);
        }

        public override void Process(float[] values, List<float> phase_errors = null, Action<float> writeSample = null)
        {
            var filtered = this.filter.Process(values);
            var demodulated = this.demodulator.Process(filtered, phase_errors, writeSample);
            this.decoder.Process(demodulated);
        }
    }
}
