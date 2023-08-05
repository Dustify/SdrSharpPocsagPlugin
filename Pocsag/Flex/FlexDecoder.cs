using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using  SdrsDecoder.Support;

namespace SdrsDecoder.Flex
{
    internal class FlexDecoder
    {
        const uint BS1 = 0b10101010101010101010101010101010;

        const uint A1 = 0b01111000111100110101100100111001;
        const uint A2 = 0b10000100111001110101100100111001;
        const uint A3 = 0b01001111100101110101100100111001;
        const uint A4 = 0b00100001010111110101100100111001;
        const uint A5 = 0b11011101010010110101100100111001;
        const uint A6 = 0b00010110001110110101100100111001;
        const uint A7 = 0b10110011100000110101100100111001;
        const uint Ar = 0b11001011001000000101100100111001;

        const uint B = 0b0101010101010101;
        // frame info

        const uint BS2 = 0b1010;
        const uint C = 0b1110110110000100;

        Dictionary<uint, string> FlexMagicValues = new Dictionary<uint, string>
        {
            { BS1, nameof(BS1) },
            { A1, nameof(A1) },
            { A2, nameof(A2) },
            { A3, nameof(A3) },
            { A4, nameof(A4) },
            { A5, nameof(A5) },
            { A6, nameof(A6) },
            { A7, nameof(A7) },
            { Ar, nameof(Ar) },
            { B, nameof(B) },
            { ~A1, nameof(A1) + "I" },
            { ~A2, nameof(A2) + "I" },
            { ~A3, nameof(A3) + "I" },
            { ~A4, nameof(A4) + "I" },
            { ~A5, nameof(A5) + "I" },
            { ~A6, nameof(A6) + "I" },
            { ~A7, nameof(A7) + "I" },
            { ~Ar, nameof(Ar) + "I" },
            { BS2, nameof(BS2) },
            { C, nameof(C) },
            { ~BS2, nameof(BS2) + "I" },
            { ~C, nameof(C) + "I" },
    };

        public List<bool> BitBuffer { get; }

        private uint bps;
        private Action<MessageBase> messageReceived;

        public FlexDecoder(uint bps, Action<MessageBase> messageReceived)
        {
            this.bps = bps;
            this.messageReceived = messageReceived;

            BitBuffer = new List<bool>();

            while (BitBuffer.Count < 32)
            {
                BitBuffer.Add(false);
            }
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

        bool inv_a_rx = false;
        uint counter = 0;

        public void BufferUpdated(uint bufferValue)
        {
            if (FlexMagicValues.ContainsKey(bufferValue))
            {
                var message =
                    new FlexMessage(bps)
                    {
                        Payload = FlexMagicValues[bufferValue],
                        Hash = DateTime.Now.ToString(),
                        Type = MessageType.AlphaNumeric,
                        HasData = true,
                        Address = "",
                        ErrorText = "",
                        HasErrors = false
                    };

                messageReceived(message);
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
