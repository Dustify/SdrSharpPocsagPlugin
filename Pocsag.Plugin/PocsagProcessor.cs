namespace Pocsag.Plugin
{
    using SDRSharp.Radio;
    using System;

    public unsafe class PocsagProcessor : IRealProcessor
    {
        public double SampleRate { get; set; }

        public bool Enabled { get; set; }

        public Manager Manager { get; set; }

        public PocsagProcessor(double sampleRate, Action<PocsagMessage> messageReceived)
        {
            try
            {
                this.SampleRate = sampleRate;

                this.Manager =
                    new Manager(
                        (int)this.SampleRate,
                        messageReceived);
            }
            catch (Exception exception)
            {
                Log.LogException(exception);
            }
        }

        public void Process(float* buffer, int length)
        {
            try
            {
                for (var i = 0; i < length; i++)
                {
                    this.Manager.Process(buffer[i]);
                }
            }
            catch (Exception exception)
            {
                Log.LogException(exception);
            }
        }
    }
}
