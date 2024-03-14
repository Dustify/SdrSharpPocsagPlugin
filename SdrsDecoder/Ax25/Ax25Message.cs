using SdrsDecoder.Support;
using System;

namespace SdrsDecoder.Ax25
{
    public class Ax25Message : MessageBase
    {
        public Ax25Message() : base(1200)
        {
            Protocol = $"AX25 / 1200";

            Hash = DateTime.Now.ToString();
            Type = MessageType.AlphaNumeric;
        }
    }
}
