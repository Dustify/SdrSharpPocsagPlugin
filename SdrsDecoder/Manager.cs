namespace SdrsDecoder
{
    using SdrsDecoder.Acars;
    using SdrsDecoder.Ax25;
    using SdrsDecoder.Flex;
    using SdrsDecoder.Pocsag;
    using SdrsDecoder.Support;
    using System;
    using System.Linq;

    public struct DecoderConfigSet
    {
        public string Name { get; set; }

        public Func<int, Action<MessageBase>, ChainBase[]> GetChains { get; set; }
    }

    public class Manager
    {
        public int SampleRate { get; }
        public Action<MessageBase> MessageReceived { get; }
        public ChainBase[] Chains { get; private set; }

        public readonly static DecoderConfigSet[] ConfigSets = {
            new DecoderConfigSet{
                Name = "POCSAG (512, 1200, 2400)",
                GetChains = (int sampleRate, Action<MessageBase> messageReceived)=>{
                    return new ChainBase[] {
                        new PocsagChain(512f, sampleRate, messageReceived),
                        new PocsagChain(1200f, sampleRate, messageReceived),
                        new PocsagChain(2400f, sampleRate, messageReceived),
                    };
                }
            },
            new DecoderConfigSet{
                Name = "FLEX (1600/2)",
                GetChains = (int sampleRate, Action<MessageBase> messageReceived)=>{
                    return new ChainBase[] {
                        new FlexChain(1600f, sampleRate, messageReceived),
                    };
                }
            },
            new DecoderConfigSet{
                Name = "AX.25 / APRS (1200)",
                GetChains = (int sampleRate, Action<MessageBase> messageReceived)=>{
                    return new ChainBase[] {
                        new Ax25Chain(sampleRate, messageReceived),
                    };
                }
            },
              new DecoderConfigSet{
                Name = "ACARS (2400)",
                GetChains = (int sampleRate, Action<MessageBase> messageReceived)=>{
                    return new ChainBase[] {
                        new AcarsChain(sampleRate, messageReceived),
                    };
                }
            },
        };

        public void ChangeMode(string mode)
        {
            try
            {
                if (ConfigSets.Count(x=>x.Name == mode)==0)
                {
                    return;
                }

                var configSet = ConfigSets.Single(x => x.Name == mode);

                this.Chains = configSet.GetChains(this.SampleRate, this.MessageReceived);
            }
            catch (Exception exception)
            {
                Log.LogException(exception);
            }
        }

        public Manager(int sampleRate, Action<MessageBase> messageReceived)
        {
            try
            {
                this.SampleRate = sampleRate;
                this.MessageReceived = messageReceived;
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
                if (this.Chains == null)
                {
                    return;
                }

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