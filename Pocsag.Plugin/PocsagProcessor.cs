namespace Pocsag.Plugin
{
    using SDRSharp.Radio;
    using System;
    using System.Runtime.InteropServices;

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
                var source = new float[length];

                Marshal.Copy((IntPtr)buffer, source, 0, length);

                this.Manager.Process(source);
                //for (var i = 0; i < length; i++)
                //{
                //    source[i] = buffer[i];

                //    this.Manager.Process(buffer[i]);
                //}
            }
            catch (Exception exception)
            {
                Log.LogException(exception);
            }
        }
    }
}
