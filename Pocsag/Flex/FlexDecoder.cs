using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using SdrsDecoder.Support;

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
        const uint BS2I = BS2 ^ 0b1111;
        const uint CI = C ^ 0b1111111111111111;

        Dictionary<uint, string> FlexAValues = new Dictionary<uint, string>
        {
            { A1, nameof(A1) },
            { A2, nameof(A2) },
            { A3, nameof(A3) },
            { A4, nameof(A4) },
            { A5, nameof(A5) },
            { A6, nameof(A6) },
            { A7, nameof(A7) },
            { Ar, nameof(Ar) }
        };

        Dictionary<uint, string> FlexAIValues = new Dictionary<uint, string>
        {
            { ~A1, nameof(A1) + "I" },
            { ~A2, nameof(A2) + "I" },
            { ~A3, nameof(A3) + "I" },
            { ~A4, nameof(A4) + "I" },
            { ~A5, nameof(A5) + "I" },
            { ~A6, nameof(A6) + "I" },
            { ~A7, nameof(A7) + "I" },
            { ~Ar, nameof(Ar) + "I" }
        };

        public BitBuffer Buffer { get; set; } = new BitBuffer();

        private uint bps;
        private Action<MessageBase> messageReceived;
        public FlexFrame Frame;

        public FlexDecoder(uint bps, Action<MessageBase> messageReceived)
        {
            this.bps = bps;
            this.messageReceived = messageReceived;

            this.Frame = new FlexFrame(messageReceived);
        }

        public int Counter = 0;

        public void BufferUpdated()
        {
            var value_32 = this.Buffer.GetValue(32);

            //if (value_32 == BS1)
            //{
            //    this.Frame.State = FrameState.SYNC1_A;

            //    return;
            //}

            if (this.Frame.State == FrameState.SYNC1_A && FlexAValues.ContainsKey(value_32))
            {
                switch (FlexAValues[value_32])
                {
                    case "A1":
                        this.Frame.Level = FlexLevel.F1600_2;
                        break;
                    case "A2":
                        this.Frame.Level = FlexLevel.F3200_2;
                        break;
                    case "A3":
                        this.Frame.Level = FlexLevel.F3200_4;
                        break;
                    case "A4":
                        this.Frame.Level = FlexLevel.F6400_4;
                        break;
                }

                this.Frame.State = FrameState.SYNC1_B;

                return;
            }

            if (this.Frame.State == FrameState.SYNC1_AI && FlexAIValues.ContainsKey(value_32))
            {
                switch (FlexAIValues[value_32])
                {
                    case "A1I":
                        this.Frame.Level = FlexLevel.F1600_2;
                        break;
                    case "A2I":
                        this.Frame.Level = FlexLevel.F3200_2;
                        break;
                    case "A3I":
                        this.Frame.Level = FlexLevel.F3200_4;
                        break;
                    case "A4I":
                        this.Frame.Level = FlexLevel.F6400_4;
                        break;
                }

                this.Frame.State = FrameState.FIW;
                this.Counter = 0;
            }

            var value_16 = this.Buffer.GetValue(16);
            var value_4 = this.Buffer.GetValue(4);

            switch (this.Frame.State)
            {
                case FrameState.SYNC1_B:
                    if (value_16 == B)
                    {
                        this.Frame.State = FrameState.SYNC1_AI;
                    }
                    break;

                case FrameState.FIW:
                    if (this.Counter < 32)
                    {
                        break;
                    }

                    this.Frame.ProcessFiw(value_32);
                    this.Frame.State = FrameState.SYNC2_BS2;

                    break;

                case FrameState.SYNC2_BS2:
                    {
                        if (value_4 == BS2)
                        {
                            this.Frame.State = FrameState.SYNC2_C;
                            //this.messageReceived(new FlexMessage(1600) { Payload = $"SYNC2_BS2" });
                        }

                        break;
                    }

                case FrameState.SYNC2_C:
                    {
                        if (value_16 == C)
                        {
                            this.Frame.State = FrameState.SYNC2_BS2I;
                            //this.messageReceived(new FlexMessage(1600) { Payload = $"SYNC2_C" });
                        }

                        break;
                    }

                case FrameState.SYNC2_BS2I:
                    {
                        if (value_4 == BS2I)
                        {
                            this.Frame.State = FrameState.SYNC2_CI;
                            //this.messageReceived(new FlexMessage(1600) { Payload = $"SYNC2_BS2I" });
                        }

                        break;
                    }

                case FrameState.SYNC2_CI:
                    {
                        if (value_16 == CI)
                        {
                            this.Frame.State = FrameState.BLOCK;
                            this.Counter = 0;
                        }

                        break;
                    }

                case FrameState.BLOCK:

                    if (this.Counter < 32)
                    {
                        break;
                    }

                    this.Counter = 0;
                    this.Frame.ProcessWord(value_32);

                    if (this.Frame.IsIdle)
                    {
                        this.Frame = new FlexFrame(this.messageReceived);
                    }

                    if (this.Frame.BlocksComplete)
                    {
                        this.Frame = new FlexFrame(this.messageReceived);

                        //this.messageReceived(new FlexMessage(1600) { Payload = $"BLOCK RESET" });
                    }

                    break;
            }

            this.Counter++;
        }

        public void Process(bool[] bits)
        {
            foreach (var bit in bits)
            {
                this.Buffer.Process(bit);

                this.BufferUpdated();
            }
        }
    }
}
