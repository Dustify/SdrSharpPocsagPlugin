namespace Pocsag
{
    using System;

    public class PocsagDecoder : DecoderBase
    {
        public int BatchIndex { get; private set; }

        public int FrameIndex { get; private set; }

        public int CodeWordInFrameIndex { get; private set; }

        public int CodeWordPosition { get; private set; }

        public PocsagMessage CurrentMessage { get; private set; }

        public PocsagDecoder(uint baud, int sampleRate, Action<PocsagMessage> messageReceived) :
            base(baud, sampleRate, messageReceived)
        {
            try
            {
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

        private void QueueCurrentMessage()
        {
            try
            {
                if (this.CurrentMessage != null &&
                    this.CurrentMessage.HasData)
                {
                    this.CurrentMessage.ProcessPayload();
                    this.MessageReceived(this.CurrentMessage);
                }

                this.CurrentMessage = new PocsagMessage(this.Bps);
            }
            catch (Exception exception)
            {
                Log.LogException(exception);
            }
        }

        public override void BufferUpdated(uint bufferValue)
        {
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
    }
}
