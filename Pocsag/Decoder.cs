namespace Pocsag
{
    using System;
    using System.Collections.Generic;

    public class Decoder
    {
        public int Baud { get; }

        public int SampleRate { get; }

        public Action<Message> MessageReceived { get; }

        public double SamplesPerBit { get; }

        public bool Value { get; private set; }

        public int SamplesForCurrentValue { get; private set; }

        public List<bool> Buffer { get; }

        public int BatchIndex { get; private set; }

        public int FrameIndex { get; private set; }

        public int CodeWordInFrameIndex { get; private set; }

        public int CodeWordPosition { get; private set; }

        public Message CurrentMessage { get; private set; }

        public Decoder(int baud, int sampleRate, Action<Message> messageReceived)
        {
            try
            {
                this.Baud = baud;
                this.SampleRate = sampleRate;

                this.MessageReceived = messageReceived;

                this.SamplesPerBit = (double)sampleRate / (double)baud;

                this.Buffer = new List<bool>();

                while (this.Buffer.Count < 32)
                {
                    this.Buffer.Add(false);
                }

                this.BatchIndex = -1;
                this.FrameIndex = -1;
                this.CodeWordInFrameIndex = -1;
                this.CodeWordPosition = -1;

                this.QueueCurrentMessage();
            }
            catch (Exception exception)
            {
                Log.LogException(exception);
            }
        }

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

        private void QueueCurrentMessage()
        {
            try
            {
                if (this.CurrentMessage != null &&
                    this.CurrentMessage.HasData &&
                    this.CurrentMessage.IsValid)
                {
                    this.CurrentMessage.ProcessPayload();
                    this.MessageReceived(this.CurrentMessage);
                }

                this.CurrentMessage = new Message(this.Baud);
            }
            catch (Exception exception)
            {
                Log.LogException(exception);
            }
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

                        // preamble
                        if (bufferValue == 0b10101010101010101010101010101010 ||
                            bufferValue == 0b01010101010101010101010101010101)
                        {
                            // reset these until we see batch sync 
                            this.BatchIndex = -1;
                            this.FrameIndex = -1;
                            this.CodeWordInFrameIndex = -1;
                            this.CodeWordPosition = -1;
                        }

                        if (this.BatchIndex > -1 &&
                            this.FrameIndex > -1 &&
                            this.CodeWordInFrameIndex > -1 &&
                            this.CodeWordPosition > -1)
                        {
                            this.CodeWordPosition++;

                            if (this.CodeWordPosition > 31)
                            {
                                this.CodeWordPosition = 0;
                                this.CodeWordInFrameIndex++;

                                // idle
                                if (bufferValue == 0b01111010100010011100000110010111)
                                {
                                                                        this.QueueCurrentMessage();
                                }
                                else
                                {
                                    // address code word? queue current message and start new message
                                    if (this.Buffer[0] == false)
                                    {
                                        this.QueueCurrentMessage();
                                    }

                                    this.CurrentMessage.AppendCodeWord(
                                        this.Buffer.ToArray(),
                                        this.FrameIndex);
                                }

                                if (this.CodeWordInFrameIndex > 1)
                                {
                                    this.CodeWordInFrameIndex = 0;
                                    this.FrameIndex++;
                                }
                            }

                            // doing this allows us to wait for batch sync below
                            if (this.FrameIndex > 7)
                            {
                                this.FrameIndex = -1;
                                this.CodeWordInFrameIndex = -1;
                                this.CodeWordPosition = -1;
                            }
                        }

                        // batch sync
                        if (bufferValue == 0b01111100110100100001010111011000)
                        {
                            this.BatchIndex++;
                            this.FrameIndex = 0;
                            this.CodeWordPosition = 0;
                            this.CodeWordInFrameIndex = 0;
                        }
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
