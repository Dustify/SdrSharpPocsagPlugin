namespace Pocsag
{
    using System;

    public class Manager
    {
        public int SampleRate { get; }

        public Decoder Decoder512 { get; }

        public Decoder Decoder1200 { get; }

        public Decoder Decoder2400 { get; }

        public Manager(int sampleRate, Action<Message> messageReceived)
        {
            try
            {
                this.SampleRate = sampleRate;

                this.Decoder512 = new Decoder(512, this.SampleRate, messageReceived);

                this.Decoder1200 = new Decoder(1200, this.SampleRate, messageReceived);

                this.Decoder2400 = new Decoder(2400, this.SampleRate, messageReceived);
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