namespace Pocsag
{
    using System;
    using System.Collections.Generic;

    public class Manager
    {
        public int SampleRate { get; }

        public List<DecoderBase> Decoders { get; }

        public Manager(int sampleRate, Action<PocsagMessage> messageReceived)
        {
            try
            {
                this.SampleRate = sampleRate;

                this.Decoders = new List<DecoderBase>();

                this.Decoders.Add(new PocsagDecoder(512, this.SampleRate, messageReceived));
                this.Decoders.Add(new PocsagDecoder(1200, this.SampleRate, messageReceived));
                this.Decoders.Add(new PocsagDecoder(2400, this.SampleRate, messageReceived));
                //this.Decoders.Add(new FlexDecoder(1600, this.SampleRate, messageReceived));
            }
            catch (Exception exception)
            {
                Log.LogException(exception);
            }
        }

        public void Process(float value)
        {
            try
            {
                foreach (var decoder in this.Decoders)
                {
                    decoder.Process(value);
                }
            }
            catch (Exception exception)
            {
                Log.LogException(exception);
            }
        }
    }
}