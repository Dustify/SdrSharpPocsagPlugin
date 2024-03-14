using SdrsDecoder.Support;
using System;

namespace SdrsDecoder.Ax25
{
    public class Ax25Decoder
    {
        private float spaceMultiplier;
        private Action<MessageBase> messageReceived;

        public Ax25Decoder(Action<MessageBase> messageReceived, float spaceMultiplier)
        {
            this.spaceMultiplier = spaceMultiplier;
            this.messageReceived = messageReceived;
        }

        public Ax25FramePost Frame = new Ax25FramePost(0, 0);

        public UInt64 Index = 0;

        public int CurrentValue = 0;

        public void Flag()
        {
            this.Frame.Process(messageReceived);
            this.Frame = new Ax25FramePost(Index, this.spaceMultiplier);
        }

        public void Process(bool value)
        {
            Index++;

            CurrentValue = CurrentValue << 1;

            if (value)
            {
                CurrentValue |= 1;
            }

            var byteValue = (byte)(CurrentValue & 0xFF);

            this.Frame.Add(byteValue);
        }

        public void Process(bool[] values)
        {
            foreach (var value in values)
            {
                this.Process(value);
            }
        }
    }
}