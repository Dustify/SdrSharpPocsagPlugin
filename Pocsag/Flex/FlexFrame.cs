using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdrsDecoder.Flex
{
    enum FrameState
    {
        INITIAL,
        SYNC1,
        FIW,
        SYNC2
    }

    internal class FlexFrame
    {
        public uint Index { get; set; } = 0;

        public FrameState State { get; set; } = FrameState.INITIAL;

        

        public void Process(bool value)
        {

        }
    }
}
