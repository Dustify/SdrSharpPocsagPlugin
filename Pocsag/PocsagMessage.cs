namespace Pocsag
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;



    public class PocsagMessage : MessageBase
    {
        public const uint Generator = 1897;

        public bool HasParityError { get; private set; }

        public string HasParityErrorText => this.HasParityError ? "Yes" : "No";

        public bool HasBchError { get; private set; }

        public string HasBchErrorText => this.HasBchError ? "Yes" : "No";

        public bool IsValid => !this.HasBchError && !this.HasParityError;

        public int ErrorsCorrected { get; private set; }

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

        }

        public bool IsBitPresent(uint source, int index)
        {
            return (source & (1 << index)) > 0;
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
                if (this.IsBitPresent(remainder, i))
                {
                    for (var x = 0; x < 11; x++)
                    {
                        var position = i - x;

                        var currentValue = this.IsBitPresent(remainder, position);
                        var generatorValue = this.IsBitPresent(Generator, 10 - x);

                        var xorResult = currentValue ^ generatorValue;

                        if (xorResult)
                        {
                            // set bit
                            remainder |= ((uint)1 << position);
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
                if (this.CheckBchError(codeWord))
                {
                    this.HasBchError = true;

                    // 1 bit error correction

                    for (var i = 0; i < codeWord.Length - 1 && this.HasBchError; i++)
                    {
                        var codeWordToCheck = (bool[])codeWord.Clone();

                        codeWordToCheck[i] = !codeWordToCheck[i];

                        if (!this.CheckBchError(codeWordToCheck))
                        {
                            codeWord = codeWordToCheck;

                            this.ErrorsCorrected++;

                            this.HasBchError = false;
                        }
                    }

                    // 2 bit error correction

                    for (var x = 0; x < codeWord.Length - 1 && this.HasBchError; x++)
                    {
                        for (var y = 0; y < codeWord.Length - 1 && this.HasBchError; y++)
                        {
                            if (x == y)
                            {
                                continue;
                            }

                            var codeWordToCheck = (bool[])codeWord.Clone();

                            codeWordToCheck[x] = !codeWordToCheck[x];
                            codeWordToCheck[y] = !codeWordToCheck[y];

                            if (!this.CheckBchError(codeWordToCheck))
                            {
                                codeWord = codeWordToCheck;

                                this.ErrorsCorrected += 2;

                                this.HasBchError = false;
                            }
                        }
                    }
                }

                this.HasData = true;

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
                        this.HasParityError = true;
                    }
                }
                else
                {
                    // parity should be one (true)
                    if (parity != true)
                    {
                        this.HasParityError = true;
                    }
                }

                // end parity check

                if (codeWord[0] == false)
                {
                    // address
                    this.FrameIndex = frameIndex;

                    this.Address += (uint)this.FrameIndex;

                    for (var i = 0; i < 18; i++)
                    {
                        var position = 20 - i;

                        if (data[i])
                        {
                            this.Address += (uint)(1 << position);
                        }
                    }

                    for (var i = 18; i < 20; i++)
                    {
                        var position = 1 - (i - 18);

                        if (data[i])
                        {
                            this.Function += (byte)(1 << position);
                        }
                    }
                }
                else
                {
                    // message
                    this.RawPayload.AddRange(data);
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

                var byteCount = (int)Math.Floor(this.RawPayload.Count / 7.0);

                for (var i = 0; i < byteCount; i++)
                {
                    var position = i * 7;

                    var currentBits =
                        this.RawPayload.
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

                this.Payload = result;

                var numericResult = string.Empty;

                var numericByteCount = (int)Math.Floor(this.RawPayload.Count / 4.0);

                for (var i = 0; i < numericByteCount; i++)
                {
                    var position = i * 4;

                    var currentBits =
                       this.RawPayload.
                       Skip(position).
                       Take(4);

                    var bitArray = new BitArray(currentBits.ToArray());

                    var byteArray = new byte[1];

                    bitArray.CopyTo(byteArray, 0);

                    numericResult += NumericMapping[byteArray[0]];
                }

                this.Type = MessageType.AlphaNumeric;

                if (numericResult.Length == 0)
                {
                    this.Payload = "";
                    this.Type = MessageType.Tone;
                }
                else if (numericResult.Length < 16)
                {
                    this.Payload = $"{numericResult} (ALPHA: {this.Payload})";
                    this.Type = MessageType.Numeric;
                }

                this.UpdateHash();
            }
            catch (Exception exception)
            {
                Log.LogException(exception);
            }
        }
    }
}
