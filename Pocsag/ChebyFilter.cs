using System;

namespace Pocsag
{
    internal class ChebyFilter
    {
        private float a0;
        private float a1;
        private float a2;
        private float b0;
        private float b1;
        private float b2;

        float x1 = 0, x2 = 0, y1 = 0, y2 = 0;

        public ChebyFilter(float cutoffFrequency, float rippleDb, float sampleRate)
        {
            float Wc = (float)(Math.Tan(Math.PI * cutoffFrequency / sampleRate));

            // Calculate filter coefficients
            float epsilon = (float)Math.Sqrt(Math.Pow(10, rippleDb / 10) - 1);
            float v = (float)(Math.Asinh(1 / epsilon) / 2);
            float k1 = (float)Math.Sinh(v);
            float k2 = (float)Math.Cosh(v);

            this.a0 = 1 + 2 * k1 * Wc + Wc * Wc;
            this.a1 = 2 * (Wc * Wc - 1);
            this.a2 = 1 - 2 * k1 * Wc + Wc * Wc;
            this.b0 = Wc * Wc;
            this.b1 = 2 * Wc * Wc;
            this.b2 = Wc * Wc;
        }

        public float[] Process(float[] inputArray)
        {
            // Initialize the filter state
            float[] filteredData = new float[inputArray.Length];


            // Apply the filter
            for (int i = 0; i < inputArray.Length; i++)
            {
                float x0 = inputArray[i];
                float y0 = (b0 * x0 + b1 * x1 + b2 * x2 - a1 * y1 - a2 * y2) / a0;

                filteredData[i] = y0;

                x2 = x1;
                x1 = x0;
                y2 = y1;
                y1 = y0;
            }

            return filteredData;
        }
    }
}
