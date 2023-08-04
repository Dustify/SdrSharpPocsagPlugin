using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Pocsag
{
    internal class FlexDecoder
    {
        const uint BS1 = 0b10101010101010101010101010101010;

        const uint A1 = 0b01111000111100110101100100111001;
        const uint A2 = 0b10000100111001110101100100111001;
        const uint A3 = 0b01001111100101110101100100111001;
        const uint A4 = 0b00100001010111110101100100111001;
        //const uint A5 = 


        public List<bool> BitBuffer { get; }

        private uint bps;
        private Action<MessageBase> messageReceived;


        public FlexDecoder(uint bps, Action<MessageBase> messageReceived)
        {
            this.bps = bps;
            this.messageReceived = messageReceived;

            this.BitBuffer = new List<bool>();

            while (this.BitBuffer.Count < 32)
            {
                this.BitBuffer.Add(false);
            }
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

        bool inv_a_rx = false;
        uint counter = 0;

        public void BufferUpdated(uint bufferValue)
        {
            counter++;

            if (!inv_a_rx && bufferValue == 0b10000111000011001010011011000110)
            {
                inv_a_rx = true;
                counter = 0;
                return;
            }

            if (inv_a_rx && counter == 32)
            {
                var message =
                    new MessageBase(this.bps)
                    {
                        Payload = Convert.ToString(bufferValue, 2),
                        Hash = DateTime.Now.ToString(),
                        Type = MessageType.AlphaNumeric,
                        HasData = true,
                        Address = "",
                        ErrorText = "",
                        HasErrors = false,
                        Protocol = $"FLEX / {this.bps}"
                    };

                this.messageReceived(message);

                inv_a_rx = false;
                counter = 0;
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
