using System;

namespace SdrsDecoder.Support
{
    public abstract class PllBase
    {
        public abstract bool Process(float value, Action<float> writeSample = null);
    }
}
