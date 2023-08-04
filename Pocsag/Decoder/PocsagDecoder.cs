using System;
using System.Collections.Generic;
using Pocsag.Message;
using Pocsag.Support;

namespace Pocsag.Decoder
{
    internal class PocsagDecoder
    {
        public int BatchIndex { get; private set; }

        public int FrameIndex { get; private set; }

        public int CodeWordInFrameIndex { get; private set; }

        public int CodeWordPosition { get; private set; }

        public PocsagMessage CurrentMessage { get; private set; }

        public List<bool> BitBuffer { get; }

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

            BitBuffer = new List<bool>();

            while (BitBuffer.Count < 32)
            {
                BitBuffer.Add(false);
            }

            BatchIndex = -1;
            FrameIndex = -1;
            CodeWordInFrameIndex = -1;
            CodeWordPosition = -1;

            QueueCurrentMessage();
        }

        private uint GetBufferValue()
        {
            var result = default(uint);

            try
            {
                var buffer = BitBuffer.ToArray();

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

        public void BufferUpdated(uint bufferValue)
        {
            // preamble
            if (bufferValue == 0b10101010101010101010101010101010 ||
                bufferValue == 0b01010101010101010101010101010101)
            {
                // reset these until we see batch sync 
                BatchIndex = -1;
                FrameIndex = -1;
                CodeWordInFrameIndex = -1;
                CodeWordPosition = -1;
            }

            if (BatchIndex > -1 &&
                FrameIndex > -1 &&
                CodeWordInFrameIndex > -1 &&
                CodeWordPosition > -1)
            {
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
                        if (BitBuffer[0] == false)
                        {
                            QueueCurrentMessage();
                        }

                        CurrentMessage.AppendCodeWord(
                            BitBuffer.ToArray(),
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
                BatchIndex++;
                FrameIndex = 0;
                CodeWordPosition = 0;
                CodeWordInFrameIndex = 0;
            }
        }

        public void Process(bool[] bits)
        {
            foreach (var bit in bits)
            {
                BitBuffer.Add(bit);

                while (BitBuffer.Count > 32)
                {
                    BitBuffer.RemoveAt(0);
                }

                var bufferValue = GetBufferValue();

                BufferUpdated(bufferValue);
            }
        }
    }
}
