using SdrsDecoder.Support;
using System;

namespace SdrsDecoder.Ax25
{
    // public enum Ax25DecoderState
    // {
    //     FRAME,
    //     ADDRESS,
    //     CONTROL,
    //     PID,
    //     PRINT
    // }

    public class Ax25Decoder
    {
        private Action<MessageBase> messageReceived;

        public Ax25Decoder(Action<MessageBase> messageReceived)
        {
            this.messageReceived = messageReceived;
        }

        // public static byte ReverseBits(byte b)
        // {
        //     byte result = 0;

        //     for (int i = 0; i < 8; i++)
        //     {
        //         result <<= 1; // Shift the result to make room for the next bit
        //         result |= (byte)(b & 1); // Add the next bit from b
        //         b >>= 1; // Shift b to the right to get the next bit
        //     }
        //     return result;
        // }

        // public Ax25DecoderState State = Ax25DecoderState.FRAME;

        // public int BitCounter = 0;

        public Ax25FramePost Frame = new Ax25FramePost();

        int counter = 0;
        
        public void Process(NrzResponse value)
        {
            if (!value.HasValue)
            {
                return;
            }

            var reversed_value = (byte)(value.Decode & 0xFF);

            this.Frame.Add(reversed_value);

            // FRAME
            if (reversed_value == 0x7e)
            {
                this.Frame.Process(messageReceived);
                this.Frame = new Ax25FramePost();
            }

            counter++;
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