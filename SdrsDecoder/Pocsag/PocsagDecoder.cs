using SdrsDecoder.Support;
using System;

namespace SdrsDecoder.Pocsag
{
    public class PocsagDecoder
    {
        public int BatchIndex { get; private set; }

        public int FrameIndex { get; private set; }

        public int CodeWordInFrameIndex { get; private set; }

        public int CodeWordPosition { get; private set; }

        public PocsagMessage CurrentMessage { get; private set; }

        public BitBuffer Buffer { get; set; } = new BitBuffer();

        private uint bps;
        private Action<PocsagMessage> messageReceived;

        private void QueueCurrentMessage()
        {
            try
            {
                if (CurrentMessage != null &&
                    CurrentMessage.HasData)
                {
                    CurrentMessage.ProcessPayload();
                    messageReceived(CurrentMessage);
                }

                CurrentMessage = new PocsagMessage(bps);
            }
            catch (Exception exception)
            {
                Log.LogException(exception);
            }
        }

        public PocsagDecoder(uint bps, Action<PocsagMessage> messageReceived)
        {
            this.bps = bps;
            this.messageReceived = messageReceived;

            BatchIndex = -1;
            FrameIndex = -1;
            CodeWordInFrameIndex = -1;
            CodeWordPosition = -1;

            QueueCurrentMessage();
        }

        private int timeout = 0;

        public void BufferUpdated(uint bufferValue)
        {
            if (timeout >= 64 && this.CurrentMessage != null && this.CurrentMessage.HasData)
            {
                this.QueueCurrentMessage();
            }

            // preamble
            if (bufferValue == 0b10101010101010101010101010101010 ||
                bufferValue == 0b01010101010101010101010101010101)
            {
                // reset these until we see batch sync 
                BatchIndex = -1;
                FrameIndex = -1;
                CodeWordInFrameIndex = -1;
                CodeWordPosition = -1;

                timeout = 0;
            }

            if (BatchIndex > -1 &&
                FrameIndex > -1 &&
                CodeWordInFrameIndex > -1 &&
                CodeWordPosition > -1)
            {
                timeout = 0;

                CodeWordPosition++;

                if (CodeWordPosition > 31)
                {
                    CodeWordPosition = 0;
                    CodeWordInFrameIndex++;

                    // idle
                    if (bufferValue == 0b01111010100010011100000110010111)
                    {
                        QueueCurrentMessage();
                    }
                    else
                    {
                        // address code word? queue current message and start new message
                        if (this.Buffer.Buffer[0] == false)
                        {
                            QueueCurrentMessage();
                        }

                        CurrentMessage.AppendCodeWord(
                            this.Buffer.Buffer.ToArray(),
                            FrameIndex);
                    }

                    if (CodeWordInFrameIndex > 1)
                    {
                        CodeWordInFrameIndex = 0;
                        FrameIndex++;
                    }
                }

                // doing this allows us to wait for batch sync below
                if (FrameIndex > 7)
                {
                    FrameIndex = -1;
                    CodeWordInFrameIndex = -1;
                    CodeWordPosition = -1;
                }
            }

            // batch sync
            if (bufferValue == 0b01111100110100100001010111011000)
            {
                timeout = 0;

                BatchIndex++;
                FrameIndex = 0;
                CodeWordPosition = 0;
                CodeWordInFrameIndex = 0;
            }

            timeout++;
        }

        public void Process(bool[] bits)
        {
            foreach (var bit in bits)
            {
                this.Buffer.Process(bit);

                var bufferValue = this.Buffer.GetValue();

                BufferUpdated(bufferValue);
            }
        }
    }
}
