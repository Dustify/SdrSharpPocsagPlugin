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

        //private float[] overflow;

        public float[] Process(float[] values)
        {
            if (this.Value <= 1)
            {
                return values;
            }

            //if (overflow!=null)
            //{
            //    values = overflow.Concat(values).ToArray();
            //}

            var result = new float[values.Length / this.Value];

            //var overflowSet = false;

            for (var x = 0; x < result.Length; x++)
            {
                var p = x * this.Value;

                if (p > values.Length - 1)
                {
                    //overflowSet = true;
                    //overflow = values.Skip((x - 1) * this.Value).ToArray();

                    continue;
                }

                result[x] = values[p];
            }

            //if (!overflowSet)
            //{
            //    overflow = null;
            //}

            return result;
        }

        public int Value { get; }
    }
}
