namespace Pocsag
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class Manager
    {
        public int SampleRate { get; }

        public List<ChainBase> Chains { get; }

        public Manager(int sampleRate, Action<PocsagMessage> messageReceived)
        {
            try
            {
                this.SampleRate = sampleRate;

                this.Chains = new List<ChainBase>
                {
                    new PocsagChain(512, this.SampleRate, messageReceived),
                    new PocsagChain(1200, this.SampleRate, messageReceived),
                    new PocsagChain(2400, this.SampleRate, messageReceived)
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
                foreach(var chain in this.Chains)
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