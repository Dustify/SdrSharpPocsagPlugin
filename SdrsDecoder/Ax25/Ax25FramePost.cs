using SdrsDecoder.Ax25;
using SdrsDecoder.Support;
using System;
using System.Collections.Generic;
using System.IO;

namespace SdrsDecoder
{
    // public struct Ax25Address
    // {
    //     public string Call;
    //     public byte Ssid;
    //     public byte RR;
    //     public byte CRH;
    // }

    // public enum Ax25ControlType
    // {
    //     None,
    //     IShort,
    //     SShort,
    //     UShort,
    //     // ILong,
    //     // SLong
    // }

    public class Ax25FramePost
    {
        public List<bool> Bits { get; } = new List<bool>();

        public void Add(byte value)
        {
            var output = (value & 1) == 1;

            if (value == 0x7e)
            {

            }

            this.Bits.Add(output);
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

        public void Process(Action<MessageBase> messageReceived)
        {
            var count = this.Bits.Count;

            if (count < 120)
            {
                return;
            }

            var byte_check = count % 8;

            var message = "";

            if (byte_check != 0)
            {
                message += $"Byte check bad: {byte_check} ";
                //Console.WriteLine($"Byte check bad: {byte_check}");
                // return;
            }

            //Console.WriteLine($"Frame okay to process ({count / 8} bytes)");

            var bytes = new List<byte>();
            var current_byte = (byte)0;
            var bit_counter = 0;

            foreach (var bit in this.Bits)
            {
                current_byte |= (byte)(bit ? 1 : 0);
                bit_counter++;

                if (bit_counter < 8)
                {
                    current_byte = (byte)(current_byte << 1);
                }
                else
                {
                    bit_counter = 0;
                    bytes.Add(current_byte);
                    current_byte = 0;
                }
            }

            foreach (var b in bytes)
            {
                var value = ReverseBits(b);

                message += (char)(value & 0x7f);
                //var o = Convert.ToString(value, 2).PadLeft(8, '0');

                //o += " " + (char)(value & 0x7f);
                //o += " " + Convert.ToString(value, 16);

                //File.AppendAllText("out.txt", o + "\n");
            }

            var messageObj = new Ax25Message();
            messageObj.Payload = message;

            messageReceived(messageObj);
        }
    }
}