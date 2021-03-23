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

        public int FrameIndex { get; private set; }

        public int CodeWordPosition { get; private set; }

        public Message CurrentMessage { get; private set; }

        public Decoder(int baud, int sampleRate, Action<Message> messageReceived)
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

            this.FrameIndex = -1;
            this.CodeWordPosition = -1;

            this.QueueCurrentMessage();
        }

        private uint GetBufferValue()
        {
            var result = default(uint);

            var buffer = this.Buffer.ToArray();

            for (var i = 0; i < buffer.Length; i++)
            {
                if (buffer[i])
                {
                    result += (uint)(1 << buffer.Length - i - 1);
                }
            }

            return result;
        }

        private void QueueCurrentMessage()
        {
            if (this.CurrentMessage != null && this.CurrentMessage.HasData)
            {
                this.CurrentMessage.ProcessPayload();
                this.MessageReceived(this.CurrentMessage);
            }

            this.CurrentMessage = new Message(this.Baud);
        }

        public void Process(float level)
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
                    //if (bufferValue == 0b10101010101010101010101010101010 ||
                    //    bufferValue == 0b01010101010101010101010101010101)
                    //{
                    //    this.builder.Append(",PREAMBLE");
                    //}

                    if (this.FrameIndex > -1 && this.CodeWordPosition > -1)
                    {
                        this.CodeWordPosition++;

                        if (this.CodeWordPosition > 31)
                        {
                            //this.builder.Append($",ENDCODEWORD{this.FrameIndex}");

                            this.CodeWordPosition = 0;
                            this.FrameIndex++;

                            // idle
                            if (bufferValue == 0b01111010100010011100000110010111)
                            {
                                //this.builder.Append($",IDLE");

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
                        }

                        if (this.FrameIndex > 15)
                        {
                            this.FrameIndex = -1;
                            this.CodeWordPosition = -1;

                            //this.builder.Append(",ENDBATCH");
                        }
                    }

                    // framesync
                    if (bufferValue == 0b01111100110100100001010111011000)
                    {
                        //this.builder.Append(",FRAMESYNC");

                        this.FrameIndex = 0;
                        this.CodeWordPosition = 0;
                    }

                    //this.builder.AppendLine();
                }

                this.SamplesForCurrentValue = 0;
                this.Value = value;
            }
            else
            {
                this.SamplesForCurrentValue++;
            }
        }
    }
}
