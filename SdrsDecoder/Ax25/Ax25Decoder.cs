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

        public Ax25FramePost Frame = new Ax25FramePost();

        public void Process(NrzResponse value)
        {
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
                this.Frame = new Ax25FramePost();
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