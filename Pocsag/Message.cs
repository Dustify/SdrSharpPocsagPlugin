namespace Pocsag
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class Message
    {
        public const uint Generator = 1897;

        public DateTime Timestamp { get; }

        public bool HasData { get; private set; }

        public int FrameIndex { get; private set; }

        public UInt32 ChannelAccessProtocolCode { get; private set; }

        public byte Function { get; private set; }

        public int Baud { get; }

        public List<bool> RawPayload { get; private set; }

        public string Payload { get; private set; }

        public bool HasParityError { get; private set; }

        public string HasParityErrorText => this.HasParityError ? "Yes" : "No";

        public bool HasBchError { get; private set; }

        public string HasBchErrorText => this.HasBchError ? "Yes" : "No";

        public bool IsValid => !this.HasBchError && !this.HasParityError;

        public string Hash { get; private set; }

        public Message(int baud)
        {
            try
            {
                this.Baud = baud;
                this.RawPayload = new List<bool>();

                this.Timestamp = DateTime.Now;
            }
            catch (Exception exception)
            {
                Log.LogException(exception);
            }
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
                this.HasData = true;

                var data = new bool[20];

                var bch = new bool[10];

                Array.Copy(codeWord, 1, data, 0, 20);
                Array.Copy(codeWord, 21, bch, 0, 10);

                var parity = codeWord[31];

                if (this.CheckBchError(codeWord))
                {
                    this.HasBchError = true;
                }

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

                    for (var i = 0; i < 18; i++)
                    {
                        var position = 17 - i;

                        if (data[i])
                        {
                            this.ChannelAccessProtocolCode += (uint)(1 << position);
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

                var textToHash = this.Payload;

                // skip first 9 characters, typically contains time / date + another number which will mess up duplicate detection

                if (textToHash.Length > 9)
                {
                    textToHash = textToHash.Substring(8);
                }

                var bytesToHash = System.Text.Encoding.ASCII.GetBytes(textToHash);

                var sha256 = new System.Security.Cryptography.SHA256Managed();

                var hashBytes = sha256.ComputeHash(bytesToHash);

                sha256.Dispose();
                sha256 = null;

                this.Hash = BitConverter.ToString(hashBytes).Replace("-", "");
            }
            catch (Exception exception)
            {
                Log.LogException(exception);
            }
        }
    }
}
