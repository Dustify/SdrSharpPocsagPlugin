namespace Pocsag
{
    using System;

    public class Manager
    {
        public int SampleRate { get; }

        public PocsagDecoder Pocsag512 { get; }

        public PocsagDecoder Pocsag1200 { get; }

        public PocsagDecoder Pocsag2400 { get; }

        public Manager(int sampleRate, Action<Message> messageReceived)
        {
            try
            {
                this.SampleRate = sampleRate;

                this.Pocsag512 = new PocsagDecoder(512, this.SampleRate, messageReceived);

                this.Pocsag1200 = new PocsagDecoder(1200, this.SampleRate, messageReceived);

                this.Pocsag2400 = new PocsagDecoder(2400, this.SampleRate, messageReceived);
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
                this.Pocsag512.Process(value);
                this.Pocsag1200.Process(value);
                this.Pocsag2400.Process(value);
            }
            catch (Exception exception)
            {
                Log.LogException(exception);
            }
        }
    }
}