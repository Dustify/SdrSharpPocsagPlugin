using System.Linq;

namespace SdrsDecoder.Support
{
    public class Interpolator
    {
        public Interpolator(int value)
        {
            this.Value = value;
        }

        public int Value { get; }

        public float[] Process(float[] values)
        {
            if (this.Value <= 1)
            {
                return values;
            }

            var result = new float[values.Length * this.Value];

            for (var x = 0; x < values.Length; x++)
            {
                result[x * this.Value] = values[x];
            }

            return result;
        }
    }
}
