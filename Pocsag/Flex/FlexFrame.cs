using SdrsDecoder.Support;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SdrsDecoder.Flex
{
    enum FrameState
    {
        SYNC1_BS1,
        SYNC1_A,
        SYNC1_B,
        SYNC1_AI,
        FIW,
        SYNC2_BS2,
        SYNC2_C,
        SYNC2_BS2I,
        SYNC2_CI,
        BLOCK
    }

    enum FlexLevel
    {
        UNKNOWN,
        F1600_2,
        F3200_2,
        F3200_4,
        F6400_4
    }

    struct FlexAddress
    {
        public bool Short;
        public uint Address;
        public string Type;
    }

    struct FlexVector
    {
        public uint Data;
        public uint Type;
        public string TypeText;
    }

    internal class FlexFrame
    {
        public uint CycleIndex { get; set; } = 0;

        public uint FrameIndex { get; set; } = 0;

        public uint BlockIndex { get; set; } = 0;

        public FlexLevel Level { get; set; } = FlexLevel.UNKNOWN;

        public FrameState State { get; set; } = FrameState.SYNC1_A;

        public List<FlexAddress> Addresses { get; set; } = new List<FlexAddress>();

        //public List<FlexVector> Vectors { get; set; } = new List<FlexVector>();
        public uint AddressStart { get; private set; }
        public uint VectorStart { get; private set; }
        public bool BlocksComplete { get; private set; }
        public bool IsIdle { get; private set; }

        public int FirstMessageWordIndex = int.MaxValue;
        public int LastMessageWordIndex = int.MinValue;

        public int WordIndex = 0;

        uint[] WordsForBlock = new uint[8];

        public List<BchResult> Words = new List<BchResult>();

        public FlexFrame(Action<MessageBase> messageReceived)
        {
            this.messageReceived = messageReceived;
        }

        public static uint ExtractUint(uint source, int start, int end)
        {
            var result = default(uint);

            var length = end - start + 1;

            for (var i = 0; i < length; i++)
            {
                var bit = (source >> end - i) & 1u;

                result |= (bit << i);
            }

            return result;
        }

        public void ProcessFiw(uint value)
        {
            // bch / parity

            var bchResult = Bch.Process(value);
            var valueResult = bchResult.Value;

            //var parity = ExtractUint(value, 0, 0);
            //var bch = ExtractUint(value, 1, 10);
            var t = ExtractUint(valueResult, 11, 14);
            var r = ExtractUint(valueResult, 15, 15);
            var n = ExtractUint(valueResult, 16, 16);
            this.FrameIndex = ExtractUint(valueResult, 17, 23);
            this.CycleIndex = ExtractUint(valueResult, 24, 27);
            //var checksum = ExtractUint(valueResult, 28, 31);

            //this.messageReceived(new FlexMessage(1600) { Payload = $"FIW: n: {n} t: {t} r: {r}" });
            //Console.WriteLine($"{this.CycleIndex} / {this.FrameIndex}");
        }

        private Action<MessageBase> messageReceived;

        public void ProcessFrame()
        {
            this.BlocksComplete = true;

            for (var i = 0; i < this.Words.Count; i++)
            {
                var result = this.Words[i];
                var value = result.Value;

                //Console.WriteLine($"{Convert.ToString(value, 2).PadLeft(32, '0')} {result.BchErrors || result.ParityError}");

                // handle BIW
                if (i == 0)
                {
                    // IDLE
                    if (value == 0b01010000001000000000101100001100)
                    {
                        this.IsIdle = true;
                        //Console.WriteLine("IDLE");
                        return;
                    }

                    this.AddressStart = ExtractUint(value, 22, 23) + 1;
                    this.VectorStart = ExtractUint(value, 16, 21);

                    //this.messageReceived(new FlexMessage(1600) { Payload = $"BIW: a: {this.AddressStart} v: {this.VectorStart} {bchResult.ParityError || bchResult.BchErrors}" });

                    continue;
                }

                // handle EXT BIW
                if (i < this.AddressStart)
                {
                    // page 45 of arib
                    this.messageReceived(new FlexMessage(1600) { Payload = $"EXTBIW: {Convert.ToString(value, 2).PadLeft(32, '0')}" });
                    continue;
                }

                // handle address
                if (i >= this.AddressStart && i < this.VectorStart)
                {
                    var address = ExtractUint(value, 11, 31);

                    var addressStruct = new FlexAddress
                    {
                        Address = address,
                        Short = false
                    };

                    // LA2
                    if (address <= 2097150)
                    {
                        addressStruct.Short = false;
                        addressStruct.Type = "LA2";
                    }


                    // RSA
                    if (address <= 2064382)
                    {
                        addressStruct.Short = true;
                        addressStruct.Type = "RSA";
                    }

                    // OMA
                    if (address <= 2062367)
                    {
                        addressStruct.Short = true;
                        addressStruct.Type = "OMA";
                    }

                    // TA
                    if (address <= 2062351)
                    {
                        addressStruct.Short = true;
                        addressStruct.Type = "TA";
                    }

                    // NA
                    if (address <= 2062335)
                    {
                        addressStruct.Short = true;
                        addressStruct.Type = "NA";
                    }

                    // ISA
                    if (address <= 2058239)
                    {
                        addressStruct.Short = true;
                        addressStruct.Type = "ISA";
                    }


                    // RSA
                    if (address <= 2041855)
                    {
                        addressStruct.Short = true;
                        addressStruct.Type = "RSA";
                    }


                    // LA4
                    if (address <= 2031616)
                    {
                        addressStruct.Short = false;
                        addressStruct.Type = "LA4";
                    }

                    // LA3
                    if (address <= 1998848)
                    {
                        addressStruct.Short = false;
                        addressStruct.Type = "LA3";
                    }


                    // SA
                    if (address <= 1966080)
                    {
                        addressStruct.Short = true;
                        addressStruct.Type = "SA";
                    }

                    // LA1
                    if (address <= 32768)
                    {
                        addressStruct.Short = false;
                        addressStruct.Type = "LA1";
                    }

                    this.Addresses.Add(addressStruct);

                    //this.messageReceived(new FlexMessage(1600) { Payload = $"Address: {address} {result.Type} {result.Short} {bchResult.ParityError || bchResult.BchErrors}" });

                    continue;
                }

                // handle vector
                if (i >= this.VectorStart && i < this.VectorStart + this.Addresses.Count)
                {
                    // check for LA + handle differently if needed

                    var vector =
                       new FlexVector
                       {
                           Data = value,
                           Type = ExtractUint(value, 25, 27)
                       };

                    uint length = 0;
                    uint start = 0;

                    var isAlpha = false;

                    // switch to static dict?
                    switch (vector.Type)
                    {
                        case 0:
                            vector.TypeText = "SECURE MSG";
                            isAlpha = true;
                            break;
                        case 1:
                            vector.TypeText = "SHORT INS";
                            break;
                        case 2:
                            vector.TypeText = "SHORT MSG";
                            break;
                        case 3:
                            vector.TypeText = "STD NUM";
                            break;
                        case 4:
                            vector.TypeText = "SFM NUM";
                            break;
                        case 5:
                            vector.TypeText = "ALPHA";
                            isAlpha = true;
                            break;
                        case 6:
                            vector.TypeText = "HEX/BIN";
                            break;
                        case 7:
                            vector.TypeText = "NUM NUM";
                            break;
                    }

                    var relevantAddress = this.Addresses[i - (int)this.VectorStart];

                    if (isAlpha)
                    {
                        length = ExtractUint(value, 11, 17);
                        start = ExtractUint(value, 18, 24);

                        var message = "";


                        for (var x = (int)start; x < start + length; x++)
                        {
                            var messageValue = this.Words[x].Value;

                            // header
                            if (x == start)
                            {
                                var mailDrop = ExtractUint(messageValue, 11, 11);
                                var retrieval = ExtractUint(messageValue, 12, 12);
                                var messageNo = ExtractUint(messageValue, 13, 18);
                                var fragInd = ExtractUint(messageValue, 19, 20);
                                var contd = ExtractUint(messageValue, 21, 21);
                                var check = ExtractUint(messageValue, 22, 31);

                                continue;
                            }

                            // signature
                            if (x == start + 1)
                            {
                                var char2_sig = (char)ExtractUint(messageValue, 11, 17);
                                var char1_sig = (char)ExtractUint(messageValue, 18, 24);
                                var signature = ExtractUint(messageValue, 25, 31);

                                message += char1_sig;
                                message += char2_sig;

                                continue;
                            }

                            var char3 = (char)ExtractUint(messageValue, 11, 17);
                            var char2 = (char)ExtractUint(messageValue, 18, 24);
                            var char1 = (char)ExtractUint(messageValue, 25, 31);

                            message += char1;
                            message += char2;
                            message += char3;
                        }

                        this.messageReceived(
                            new FlexMessage(1600)
                            {
                                Address = relevantAddress.Address.ToString(),
                                Payload = message
                            }
                        );
                    }
                    else
                    {
                        this.messageReceived(new FlexMessage(1600) { Payload = $"Vector: {vector.TypeText} S: {start} L: {length}" });
                    }

                    continue;
                }
            }
        }

        public void ProcessBlock()
        {
            var realWords = new uint[8];

            var counter = 0;

            foreach (var word in this.WordsForBlock)
            {
                for (var i = 0; i < 32; i++)
                {
                    var bit = (word >> 31 - i) & 1u;

                    var realWordIndex = counter % 8;
                    var bitToUpdate = (counter - realWordIndex) / 8;

                    realWords[realWordIndex] |= (bit << 31 - bitToUpdate);

                    counter++;
                }
            }

            var errors = 0;

            foreach (var realWord in realWords)
            {
                var result = Bch.Process(realWord);

                // consider stopping here if value == 0?

                if (result.Value == 0)
                {
                    this.ProcessFrame();

                    return;
                }

                if (result.BchErrors || result.ParityError)
                {
                    errors++;
                }

                this.Words.Add(result);
            }

            //if (errors > 4)
            //{
            //    this.ProcessFrame();
            //}
        }

        public void ProcessWord(uint value)
        {
            // frame complete, stop processing
            if (this.BlockIndex > 10)
            {
                return;
            }

            this.WordsForBlock[this.WordIndex] = value;

            this.WordIndex++;

            // check if block data complete
            if (this.WordIndex >= 8)
            {
                this.ProcessBlock();

                if (this.BlockIndex == 10)
                {
                    this.ProcessFrame();
                    return;
                }

                this.WordIndex = 0;
                this.BlockIndex++;
            }
        }
    }
}
