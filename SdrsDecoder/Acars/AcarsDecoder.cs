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

        public void Reset()
        {
            this.Bits = new List<bool>();
        }

        public void Flag()
        {
            if (this.Bits.Count < 40)
            {
                this.Reset();
                return;
            }

            if (this.Bits.Count % 8 != 0)
            {
                this.Reset();
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

            // make sure 3rd from last is ETX / ETB
            var hasEomError = true;

            var eomByte = bytes[bytes.Count - 3] & 0b01111111;

            if (eomByte == 3 || eomByte == 23)
            {
                hasEomError = false;
            }

            var hasParityError = false;
            var hasCrcError = false;

            var rxCrc = (ushort)(bytes[^2] | (bytes[^1] << 8));
            var calcCrc = (ushort)0;

            for (var i = 0; i < bytes.Count - 2; i++)
            {
                var b = bytes[i];
                var rxParity = (b >> 7) == 1;
                var calcParity = false;

                // crc + parity

                calcCrc ^= b;

                for (var y = 0; y < 8; y++)
                {
                    if ((calcCrc & 1) == 1)
                    {
                        calcCrc >>= 1;
                        calcCrc ^= 0x8408;
                    }
                    else
                    {
                        calcCrc >>= 1;
                    }

                    if (y >= 7)
                    {
                        continue;
                    }

                    if ((b >> y & 1) == 0)
                    {
                        calcParity = !calcParity;
                    }
                }

                if (rxParity != calcParity)
                {
                    hasParityError = true;
                }

                if (i >= bytes.Count - 3)
                {
                    continue;
                }

                // get char for payload
                var v = b & 0b01111111;

                payload += (char)v;
            }

            if (rxCrc != calcCrc)
            {
                hasCrcError = true;
            }

            // process data
            var message = new AcarsMessage();

            message.HasErrors = hasParityError || hasEomError || hasCrcError;
            message.ErrorText = message.HasErrors ? "Yes" : "No";
            message.Payload = payload;

            if (message.HasErrors)
            {
                message.ErrorText += " (";

                if (hasParityError)
                {
                    message.ErrorText += "P";
                }

                if (hasEomError)
                {
                    message.ErrorText += "E";
                }

                if (hasCrcError)
                {
                    message.ErrorText += "C";
                }

                message.ErrorText += ")";
            }

            this.messageReceived(message);

            this.Reset();
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
