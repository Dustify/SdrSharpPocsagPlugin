namespace SdrsDecoder.Pocsag
{
    using SdrsDecoder.Support;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class PocsagMessage : MessageBase
    {
        public const uint Generator = 1897;

        public List<bool> RawPayload { get; set; }

        public static readonly Dictionary<byte, char> NumericMapping =
            new Dictionary<byte, char>()
            {
                { 0, '0' },
                { 1, '1' },
                { 2, '2' },
                { 3, '3' },
                { 4, '4' },
                { 5, '5' },
                { 6, '6' },
                { 7, '7' },
                { 8, '8' },
                { 9, '9' },
                { 10, '?' },
                { 11, 'U' },
                { 12, ' ' },
                { 13, '-' },
                { 14, ')' },
                { 15, '(' }
            };

        public PocsagMessage(uint bps) : base(bps)
        {
            RawPayload = new List<bool>();
            Protocol = $"POCSAG / {bps}";
        }

        public bool IsBitPresent(uint source, int index)
        {
            return (source & 1 << index) > 0;
        }

        public bool CheckBchError(bool[] codeWord)
        {
            // BCH / error correction

            // converts bool array to uint
            uint codeWordAsUInt = 0;

            for (var i = 0; i < 31; i++)
            {
                if (codeWord[i])
                {
                    codeWordAsUInt += (uint)(1 << 30 - i);
                }
            }

            // modulo 2 division

            uint remainder = codeWordAsUInt;

            for (var i = 30; i >= 0; i--)
            {
                // if bit is set then do xor
                if (IsBitPresent(remainder, i))
                {
                    for (var x = 0; x < 11; x++)
                    {
                        var position = i - x;

                        var currentValue = IsBitPresent(remainder, position);
                        var generatorValue = IsBitPresent(Generator, 10 - x);

                        var xorResult = currentValue ^ generatorValue;

                        if (xorResult)
                        {
                            // set bit
                            remainder |= (uint)1 << position;
                        }
                        else
                        {
                            // clear bit
                            remainder &= ~((uint)1 << position);
                        }
                    }
                }
            }

            return remainder > 0;
        }

        public void AppendCodeWord(bool[] codeWord, int frameIndex)
        {
            try
            {
                var errors = false;

                if (CheckBchError(codeWord))
                {
                    errors = true;

                    // 1 bit error correction

                    for (var i = 0; i < codeWord.Length - 1 && HasErrors; i++)
                    {
                        var codeWordToCheck = (bool[])codeWord.Clone();

                        codeWordToCheck[i] = !codeWordToCheck[i];

                        if (!CheckBchError(codeWordToCheck))
                        {
                            codeWord = codeWordToCheck;

                            //this.ErrorsCorrected++;

                            errors = false;
                        }
                    }

                    // 2 bit error correction

                    for (var x = 0; x < codeWord.Length - 1 && HasErrors; x++)
                    {
                        for (var y = 0; y < codeWord.Length - 1 && HasErrors; y++)
                        {
                            if (x == y)
                            {
                                continue;
                            }

                            var codeWordToCheck = (bool[])codeWord.Clone();

                            codeWordToCheck[x] = !codeWordToCheck[x];
                            codeWordToCheck[y] = !codeWordToCheck[y];

                            if (!CheckBchError(codeWordToCheck))
                            {
                                codeWord = codeWordToCheck;

                                //this.ErrorsCorrected += 2;

                                errors = false;
                            }
                        }
                    }
                }

                HasData = true;

                var data = new bool[20];

                var bch = new bool[10];

                Array.Copy(codeWord, 1, data, 0, 20);
                Array.Copy(codeWord, 21, bch, 0, 10);

                var parity = codeWord[31];

                // start parity check
                var trueCount = codeWord.Take(31).Count(x => x == true);

                if (trueCount % 2 == 0)
                {
                    // parity should be zero (false)
                    if (parity != false)
                    {
                        errors = true;
                    }
                }
                else
                {
                    // parity should be one (true)
                    if (parity != true)
                    {
                        errors = true;
                    }
                }

                if (!HasErrors && errors)
                {
                    HasErrors = true;
                }

                ErrorText = HasErrors ? "Yes" : "No";

                // end parity check

                if (codeWord[0] == false)
                {
                    var address = default(uint);
                    // address
                    //this.FrameIndex = frameIndex;

                    address += (uint)frameIndex;

                    for (var i = 0; i < 18; i++)
                    {
                        var position = 20 - i;

                        if (data[i])
                        {
                            address += (uint)(1 << position);
                        }
                    }

                    var function = default(byte);

                    for (var i = 18; i < 20; i++)
                    {
                        var position = 1 - (i - 18);

                        if (data[i])
                        {
                            function += (byte)(1 << position);
                        }
                    }

                    Address = $"{address} / {function}";
                }
                else
                {
                    // message
                    RawPayload.AddRange(data);
                }
            }
            catch (Exception exception)
            {
                Log.LogException(exception);
            }
        }

        public void ProcessPayload()
        {
            try
            {
                var result = string.Empty;

                var byteCount = (int)Math.Floor(RawPayload.Count / 7.0);

                for (var i = 0; i < byteCount; i++)
                {
                    var position = i * 7;

                    var currentBits =
                        RawPayload.
                        Skip(position).
                        Take(7);

                    var bitArray = new BitArray(currentBits.ToArray());

                    var byteArray = new byte[1];

                    bitArray.CopyTo(byteArray, 0);

                    if (byteArray[0] != 0)
                    {
                        result += (char)byteArray[0];
                    }
                }

                Payload = result;

                var numericResult = string.Empty;

                var numericByteCount = (int)Math.Floor(RawPayload.Count / 4.0);

                for (var i = 0; i < numericByteCount; i++)
                {
                    var position = i * 4;

                    var currentBits =
                       RawPayload.
                       Skip(position).
                       Take(4);

                    var bitArray = new BitArray(currentBits.ToArray());

                    var byteArray = new byte[1];

                    bitArray.CopyTo(byteArray, 0);

                    numericResult += NumericMapping[byteArray[0]];
                }

                Type = MessageType.AlphaNumeric;

                if (numericResult.Length == 0)
                {
                    Payload = "";
                    Type = MessageType.Tone;
                }
                else if (numericResult.Length < 16)
                {
                    Payload = $"{numericResult} (ALPHA: {Payload})";
                    Type = MessageType.Numeric;
                }

                UpdateHash();
            }
            catch (Exception exception)
            {
                Log.LogException(exception);
            }
        }
    }
}
