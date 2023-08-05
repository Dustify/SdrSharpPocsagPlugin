using System;
using System.Collections.Generic;

namespace SdrsDecoder.Support
{
    public abstract class ChainBase
    {
        protected float sampleRate;
        protected Action<MessageBase> messageReceived;

        public ChainBase(float sampleRate, Action<MessageBase> messageReceived)
        {
            this.sampleRate = sampleRate;
            this.messageReceived = messageReceived;
        }

        public abstract void Process(float[] values, List<float> phase_errors = null, Action<float> writeSample = null);
    }
}
