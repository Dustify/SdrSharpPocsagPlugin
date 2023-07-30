using System;
using System.Collections.Generic;

namespace Pocsag
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
                if (this.CurrentMessage != null &&
                    this.CurrentMessage.HasData)
                {
                    this.CurrentMessage.ProcessPayload();
                    this.messageReceived(this.CurrentMessage);
                }

                this.CurrentMessage = new PocsagMessage(this.bps);
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

            this.BitBuffer = new List<bool>();

            while (this.BitBuffer.Count < 32)
            {
                this.BitBuffer.Add(false);
            }

            this.BatchIndex = -1;
            this.FrameIndex = -1;
            this.CodeWordInFrameIndex = -1;
            this.CodeWordPosition = -1;

            this.QueueCurrentMessage();
        }

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

        public void BufferUpdated(uint bufferValue)
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
                        if (this.BitBuffer[0] == false)
                        {
                            this.QueueCurrentMessage();
                        }

                        this.CurrentMessage.AppendCodeWord(
                            this.BitBuffer.ToArray(),
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

        public void Process(bool[] bits)
        {
            foreach (var bit in bits)
            {
                this.BitBuffer.Add(bit);

                while (this.BitBuffer.Count > 32)
                {
                    this.BitBuffer.RemoveAt(0);
                }

                var bufferValue = this.GetBufferValue();

                this.BufferUpdated(bufferValue);
            }
        }
    }
}
