namespace SdrsDecoder
{
    using SdrsDecoder.Ax25;
    using SdrsDecoder.Flex;
    using SdrsDecoder.Pocsag;
    using SdrsDecoder.Support;
    using System;
    using System.Collections.Generic;

    public class Manager
    {
        public int SampleRate { get; }

        public List<ChainBase> Chains { get; }

        public Manager(int sampleRate, Action<MessageBase> messageReceived)
        {
            try
            {
                this.SampleRate = sampleRate;

                this.Chains = new List<ChainBase>
                {
                    new PocsagChain(512f, this.SampleRate, messageReceived),
                    new PocsagChain(1200f, this.SampleRate, messageReceived),
                    new PocsagChain(2400f, this.SampleRate, messageReceived),
                    new FlexChain(1600f, this.SampleRate, messageReceived),
                    //new FlexChain(3200f, this.SampleRate, messageReceived),
                    new Ax25Chain(this.SampleRate,messageReceived)
            };
            }
            catch (Exception exception)
            {
                Log.LogException(exception);
            }
        }

        public void Process(float[] values)
        {
            try
            {
                foreach (var chain in this.Chains)
                {
                    chain.Process(values);
                }
            }
            catch (Exception exception)
            {
                Log.LogException(exception);
            }
        }
    }
}