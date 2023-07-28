namespace Pocsag
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class Manager
    {
        public int SampleRate { get; }

        public List<DecoderBase> Decoders { get; }

        public int Pocsag512FilterDepth
        {
            get
            {
                return this.Decoders[0].FilterDepth;
            }
            set
            {
                this.Decoders[0].FilterDepth = value;
            }
        }

        public int Pocsag1200FilterDepth
        {
            get
            {
                return this.Decoders[1].FilterDepth;
            }
            set
            {
                this.Decoders[1].FilterDepth = value;
            }
        }

        public int Pocsag2400FilterDepth
        {
            get
            {
                return this.Decoders[2].FilterDepth;
            }
            set
            {
                this.Decoders[2].FilterDepth = value;
            }
        }

        public Manager(int sampleRate, Action<PocsagMessage> messageReceived)
        {
            try
            {
                this.SampleRate = sampleRate;

                this.Decoders = new List<DecoderBase>();

                this.Decoders.Add(new PocsagDecoder(512, this.SampleRate, messageReceived));
                this.Decoders.Add(new PocsagDecoder(1200, this.SampleRate, messageReceived));
                this.Decoders.Add(new PocsagDecoder(2400, this.SampleRate, messageReceived));
                //this.Decoders.Add(new FlexDecoder(1600, this.SampleRate, messageReceived));
            }
            catch (Exception exception)
            {
                Log.LogException(exception);
            }
        }

        public void Process(float value)
        {
            try
            {
                foreach (var decoder in this.Decoders)
                {
                    decoder.Process(value);
                }
            }
            catch (Exception exception)
            {
                Log.LogException(exception);
            }
        }

        public float[] ApplyChebyshevFilter(float[] inputArray, float cutoffFrequency, float rippleDb)
        {
            float Wc = (float)(Math.Tan(Math.PI * cutoffFrequency / this.SampleRate));

            // Calculate filter coefficients
            float epsilon = (float)Math.Sqrt(Math.Pow(10, rippleDb / 10) - 1);
            float v = (float)(Math.Asinh(1 / epsilon) / 2);
            float k1 = (float)Math.Sinh(v);
            float k2 = (float)Math.Cosh(v);

            float a0 = 1 + 2 * k1 * Wc + Wc * Wc;
            float a1 = 2 * (Wc * Wc - 1);
            float a2 = 1 - 2 * k1 * Wc + Wc * Wc;
            float b0 = Wc * Wc;
            float b1 = 2 * Wc * Wc;
            float b2 = Wc * Wc;

            // Initialize the filter state
            float[] filteredData = new float[inputArray.Length];
            float x1 = 0, x2 = 0, y1 = 0, y2 = 0;

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

        public float[] ApplyButterworthFilter(float[] inputArray, float cutoffFrequency, int order)
        {
            // Validate order
            if (order < 1)
            {
                throw new ArgumentException("Filter order must be at least 1.");
            }

            // Calculate coefficients for the 1st order Butterworth low-pass filter
            float dt = 1.0f / this.SampleRate;
            float rc = 1.0f / (2 * (float)Math.PI * cutoffFrequency);
            float alpha = dt / (rc + dt);

            // Initialize the filter state
            float[] filteredData = new float[inputArray.Length];
            filteredData[0] = inputArray[0];

            // Apply the cascaded 1st order Butterworth filters
            for (int o = 0; o < order; o++)
            {
                for (int i = 1; i < inputArray.Length; i++)
                {
                    filteredData[i] = filteredData[i - 1] + alpha * (inputArray[i] - filteredData[i - 1]);
                }
                // Set input for the next filter in the cascade
                inputArray = filteredData;
            }

            return filteredData;
        }

        public void Process(float[] values)
        {
            try
            {
                var filtered = this.ApplyChebyshevFilter(values, 4800f, 0.5f);
                //var filtered = this.ApplyButterworthFilter(values, 4800f, 2);

                foreach (var value in filtered)
                {
                    this.Process(value);
                }
            }
            catch (Exception exception)
            {
                Log.LogException(exception);
            }
        }


    }
}