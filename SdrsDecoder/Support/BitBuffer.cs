using System.Collections.Generic;
using System.Linq;

namespace SdrsDecoder.Support
{
    public class BitBuffer
    {
        public List<bool> Buffer = new List<bool>();

        public int Length { get; }

        public BitBuffer()
        {
            for (int i = 0; i < 32; i++)
            {
                this.Buffer.Add(false);
            }
        }

        public void Process(bool value)
        {
            this.Buffer.Add(value);

            while (this.Buffer.Count > 32)
            {
                this.Buffer.RemoveAt(0);
            }
        }

        public uint GetValue(int length = 32)
        {
            var result = default(uint);

            var skip = this.Buffer.Count - length;

            var values = this.Buffer.Skip(skip).Take(length).ToArray();

            for (var i = 0; i < length; i++)
            {
                if (values[i])
                {
                    result += (uint)(1 << length - i - 1);
                }
            }

            return result;
        }
    }
}
