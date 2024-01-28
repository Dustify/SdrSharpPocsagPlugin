using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdrsDecoder.Support
{
    public class Decimator
    {
        public Decimator(int value)
        {
            this.Value = value;
        }

        public float[] Process(float[] values)
        {
            if (this.Value <= 1)
            {
                return values;
            }

            var result = new float[values.Length / this.Value];

            for (var x = 0; x < result.Length; x++)
            {
                var p = x * this.Value;

                if (p > values.Length - 1)
                {
                    continue;
                }

                result[x] = values[p];
            }

            return result;
        }

        public int Value { get; }
    }
}
