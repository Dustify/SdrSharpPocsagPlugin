namespace Pocsag
{
    using System;

    public class Manager
    {
        public int SampleRate { get; }

        public PocsagDecoder Decoder512 { get; }

        public PocsagDecoder Decoder1200 { get; }

        public PocsagDecoder Decoder2400 { get; }

        public Manager(int sampleRate, Action<Message> messageReceived)
        {
            try
            {
                this.SampleRate = sampleRate;

                this.Decoder512 = new PocsagDecoder(512, this.SampleRate, messageReceived);

                this.Decoder1200 = new PocsagDecoder(1200, this.SampleRate, messageReceived);

                this.Decoder2400 = new PocsagDecoder(2400, this.SampleRate, messageReceived);
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
                this.Decoder512.Process(value);
                this.Decoder1200.Process(value);
                this.Decoder2400.Process(value);
            }
            catch (Exception exception)
            {
                Log.LogException(exception);
            }
        }
    }
}