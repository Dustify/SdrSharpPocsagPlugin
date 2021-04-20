namespace Pocsag
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    public abstract class DecoderBase
    {
        public Action<PocsagMessage> MessageReceived { get; }

        public uint Bps { get; }

        public int SampleRate { get; }

        public double SamplesPerBit { get; }

        public List<bool> BitBuffer { get; }

        public bool Value { get; private set; }

        public int SamplesForCurrentValue { get; private set; }

        public abstract int FilterDepth { get; }

        public DecoderBase(uint baud, int sampleRate, Action<PocsagMessage> messageReceived)
        {
            this.Bps = baud;
            this.SampleRate = sampleRate;

            this.MessageReceived = messageReceived;

            this.SamplesPerBit = (double)sampleRate / (double)baud;

            this.BitBuffer = new List<bool>();
        }

        public abstract void BufferUpdated(uint bufferValue);

        private uint GetBufferValue()
        {
            var result = default(uint);

            try
            {
                var buffer = this.BitBuffer.ToArray();

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

        public List<float> Filter = new List<float>();

        public void Process(float level)
        {
            try
            {
                this.Filter.Add(level);

                while (this.Filter.Count > this.FilterDepth )
                {
                    this.Filter.RemoveAt(0);
                }

                var filteredLevel = this.Filter.Average();

                //get current state
                var value = filteredLevel < 0;

                // has stage changed? zero crossing
                if (value != this.Value)
                {
                    var bitsForPreviousValue = (double)this.SamplesForCurrentValue / this.SamplesPerBit;

                    var bitsForPreviousValueRounded = Math.Round(bitsForPreviousValue);

                    for (var i = 0; i < (int)bitsForPreviousValueRounded; i++)
                    {
                        // add bits as state we have changed from
                        this.BitBuffer.Add(this.Value);

                        // remove old items from buffer
                        while (this.BitBuffer.Count > 32)
                        {
                            this.BitBuffer.RemoveAt(0);
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
