using System;

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

            this.filter = new ChebyFilter(this.bps * 3f, 1f, this.sampleRate);
            this.demodulator = new Fsk2Demodulator(this.bps, this.sampleRate);
            this.decoder = new PocsagDecoder(Convert.ToUInt32(this.bps), messageReceived);
        }

        public override void Process(float[] values)
        {
            var filtered = this.filter.Process(values);
            var demodulated = this.demodulator.Process(filtered);
            this.decoder.Process(demodulated);
        }
    }
}
