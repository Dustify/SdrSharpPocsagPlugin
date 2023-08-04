using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pocsag.Message
{
    internal class FlexMessage : MessageBase
    {
        public FlexMessage(uint bps) : base(bps)
        {
            this.Protocol = $"FLEX / {bps} / 2";

            this.Hash = DateTime.Now.ToString();
            this.Type = MessageType.AlphaNumeric;
        }
    }
}
