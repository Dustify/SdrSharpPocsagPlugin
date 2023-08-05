using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using  SdrsDecoder.Support;

namespace SdrsDecoder.Flex
{
    internal class FlexMessage : MessageBase
    {
        public FlexMessage(uint bps) : base(bps)
        {
            Protocol = $"FLEX / {bps} / 2";

            Hash = DateTime.Now.ToString();
            Type = MessageType.AlphaNumeric;
        }
    }
}
