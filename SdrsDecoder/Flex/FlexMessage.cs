using SdrsDecoder.Support;
using System;

namespace SdrsDecoder.Flex
{
    public class FlexMessage : MessageBase
    {
        public FlexMessage(uint bps) : base(bps)
        {
            Protocol = $"FLEX / {bps} / 2";

            Hash = DateTime.Now.ToString();
            Type = MessageType.AlphaNumeric;
        }
    }
}
