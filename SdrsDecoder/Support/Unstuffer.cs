using System.Collections.Generic;

namespace SdrsDecoder.Support
{
    public class Unstuffer
    {
        public uint CurrentValue = 0;

        public bool? Process(bool value)
        {
            CurrentValue = CurrentValue << 1;

            if (value)
            {
                CurrentValue |= 1;
            }

            if ((CurrentValue & 0x3f) == 0x3e)
            {
                return null;
            }

            return value;
        }

        public bool[] Process(bool[] values)
        {
            var result = new List<bool>();

            foreach (var v in values)
            {
                var r = this.Process(v);

                if (!r.HasValue)
                {
                    continue;
                }

                result.Add(r.Value);
            }

            return result.ToArray();
        }
    }
}
