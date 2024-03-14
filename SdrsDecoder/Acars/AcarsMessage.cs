using SdrsDecoder.Support;
using System;

namespace SdrsDecoder.Acars
{
    public class AcarsMessage : MessageBase
    {
        public AcarsMessage() : base(2400)
        {
            Protocol = $"ACARS / 2400";

            Hash = DateTime.Now.ToString();
            Type = MessageType.AlphaNumeric;
        }
    }
}
