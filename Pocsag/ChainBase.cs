using System;
using System.Collections.Generic;

namespace Pocsag
{
    public abstract class ChainBase
    {
        protected float sampleRate;
        protected Action<PocsagMessage> messageReceived;

        public ChainBase(float sampleRate, Action<PocsagMessage> messageReceived) {
            this.sampleRate = sampleRate;
            this.messageReceived = messageReceived;
        }

        public abstract void Process(float[] values, List<float> phase_errors = null, Action<float> writeSample = null);
    }
}
