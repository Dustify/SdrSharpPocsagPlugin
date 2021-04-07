namespace Pocsag
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Collections;

    public class Message
    {
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

                // BCH / error correction
                // too hard for the moment, will probably need some help!

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
            }
            catch (Exception exception)
            {
                Log.LogException(exception);
            }
        }
    }
}
