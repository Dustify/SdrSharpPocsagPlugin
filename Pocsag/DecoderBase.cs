namespace Pocsag
{
    using System;
    using System.Collections.Generic;

    public abstract class DecoderBase
    {
        public Action<Message> MessageReceived { get; }

        public int Baud { get; }

        public int SampleRate { get; }

        public double SamplesPerBit { get; }

        public List<bool> Buffer { get; }

        public bool Value { get; private set; }

        public int SamplesForCurrentValue { get; private set; }

        public DecoderBase(int baud, int sampleRate, Action<Message> messageReceived)
        {
            this.Baud = baud;
            this.SampleRate = sampleRate;

            this.MessageReceived = messageReceived;

            this.SamplesPerBit = (double)sampleRate / (double)baud;

            this.Buffer = new List<bool>();
        }

        public abstract void BufferUpdated(uint bufferValue);

        private uint GetBufferValue()
        {
            var result = default(uint);

            try
            {
                var buffer = this.Buffer.ToArray();

                for (var i = 0; i < buffer.Length; i++)
                {
                    if (buffer[i])
                    {
                        result += (uint)(1 << buffer.Length - i - 1);
                    }
                }
            }
            catch (Exception exception)
            {
                Log.LogException(exception);
            }

            return result;
        }

        public void Process(float level)
        {
            try
            {
                // get current state
                var value = level < 0;

                // has stage changed? zero crossing
                if (value != this.Value)
                {
                    var bitsForPreviousValue = (double)this.SamplesForCurrentValue / SamplesPerBit;
                    var bitsForPreviousValueRounded = (int)Math.Round(bitsForPreviousValue);

                    for (var i = 0; i < bitsForPreviousValueRounded; i++)
                    {
                        // add bits as state we have changed from
                        this.Buffer.Add(this.Value);

                        // remove old items from buffer
                        while (this.Buffer.Count > 32)
                        {
                            this.Buffer.RemoveAt(0);
                        }

                        var bufferValue = this.GetBufferValue();

                        this.BufferUpdated(bufferValue);
                    }

                    this.SamplesForCurrentValue = 0;
                    this.Value = value;
                }
                else
                {
                    this.SamplesForCurrentValue++;
                }

            }
            catch (Exception exception)
            {
                Log.LogException(exception);
            }
        }
    }
}
