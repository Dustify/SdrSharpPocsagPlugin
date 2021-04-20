namespace Pocsag
{
    using System;

    public class FlexDecoder : DecoderBase
    {
        public FlexDecoder(uint baud, int sampleRate, Action<PocsagMessage> messageReceived) :
            base(baud, sampleRate, messageReceived)
        {

        }

        public override void BufferUpdated(uint bufferValue)
        {
            if (bufferValue == 0b10101010101010101010101010101010 ||
               bufferValue == 0b01010101010101010101010101010101)
            {
                
            }
        }
    }
}
