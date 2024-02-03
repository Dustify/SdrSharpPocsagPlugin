using SdrsDecoder.Support;
using System;

namespace SdrsDecoder.Ax25
{
    public class Ax25Decoder
    {
        private Action<MessageBase> messageReceived;

        public Ax25Decoder(Action<MessageBase> messageReceived)
        {
            this.messageReceived = messageReceived;
        }

        public Ax25FramePost Frame = new Ax25FramePost(0);

        public UInt64 Index = 0;

        public void Process(NrzResponse value)
        {
            Index++;

            if (!value.HasValue)
            {
                return;
            }

            // inverted not reversed?
            var reversed_value = (byte)(value.Decode & 0xFF);

            this.Frame.Add(reversed_value);

            // FRAME
            if (reversed_value == 0x7e)
            {
                this.Frame.Process(messageReceived);
                this.Frame = new Ax25FramePost(Index);
            }
        }

        public void Process(NrzResponse[] values)
        {
            foreach (var value in values)
            {
                this.Process(value);
            }
        }
    }
}