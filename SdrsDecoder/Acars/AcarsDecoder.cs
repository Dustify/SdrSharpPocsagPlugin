using SdrsDecoder.Support;
using System;
using System.Collections.Generic;

namespace SdrsDecoder.Acars
{
    public class AcarsDecoder
    {
        public AcarsDecoder(Action<MessageBase> messageReceived)
        {
            this.messageReceived = messageReceived;
        }

        public static byte ReverseBits(byte b)
        {
            byte result = 0;

            for (int i = 0; i < 8; i++)
            {
                result <<= 1; // Shift the result to make room for the next bit
                result |= (byte)(b & 1); // Add the next bit from b
                b >>= 1; // Shift b to the right to get the next bit
            }
            return result;
        }

        public List<bool> Bits = new List<bool>();

        public void Flag()
        {
            if (this.Bits.Count % 8 != 0)
            {
                this.Bits = new List<bool>();
                return;
            }

            // remove DEL at end
            this.Bits.RemoveRange(this.Bits.Count - 8, 8);

            var bytes = new List<byte>();
            var current_byte = (byte)0;
            var bit_counter = 0;

            for (var i = 0; i < this.Bits.Count; i++)
            {
                var bit = this.Bits[i];
                current_byte |= (byte)(bit ? 1 : 0);
                bit_counter++;

                if (bit_counter < 8)
                {
                    current_byte = (byte)(current_byte << 1);
                }
                else
                {
                    bit_counter = 0;

                    var newByte = ReverseBits(current_byte);
                    bytes.Add(newByte);

                    current_byte = 0;
                }
            }



            var payload = "";

            // end of message

            // ETX or ETB
            // CRC pt1
            // CRC pt2
            // DEL (already removed)


            var hasParityError = false;


            for (var i = 0; i < bytes.Count - 2; i++)
            {
                var b = bytes[i];
                var v = b & 0b01111111;

                payload += (char)v;

                var rxParity = (b >> 7) == 1;
                var calcParity = false;

                for (var y = 0; y < 7; y++)
                {
                    if ((b >> y & 1) == 0)
                    {
                        calcParity = !calcParity;
                    }
                }

                if (rxParity != calcParity)
                {
                    hasParityError = true;
                }
            }

            // CRC needed here

            // process data
            var message = new AcarsMessage();

            message.HasErrors = hasParityError;
            message.Payload = payload;

            this.messageReceived(message);

            this.Bits = new List<bool>();
        }

        int currentValue;
        private Action<MessageBase> messageReceived;

        public void Process(bool value)
        {
            this.Bits.Add(value);

            currentValue = currentValue << 1;

            if (value)
            {
                currentValue |= 1;
            }

            if (this.Bits.Count % 8 != 0)
            {
                return;
            }

            if ((currentValue & 0xff) == 0b11111110)
            {
                this.Flag();
            }
        }
    }
}
