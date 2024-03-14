using SdrsDecoder.Support;
using System;

namespace SdrsDecoder
{

    public class IqDemod
    {
        public float[] SpaceMultiplers;

        public float[] MarkI { get; }
        public float[] MarkQ { get; }
        public float[] SpaceI { get; }
        public float[] SpaceQ { get; }
        public FixedSizeQueue<float> Buffer { get; }

        public IqDemod(float sampleRate, float baud, float mark, float space, float[] spaceMultipliers)
        {
            this.SpaceMultiplers = spaceMultipliers;

            var samps_per_symbol = (int)Math.Round(sampleRate / baud);

            this.MarkI = new float[samps_per_symbol];
            this.MarkQ = new float[samps_per_symbol];
            this.SpaceI = new float[samps_per_symbol];
            this.SpaceQ = new float[samps_per_symbol];

            var mark_inc = mark / sampleRate * Math.PI * 2;
            var space_inc = space / sampleRate * Math.PI * 2;

            for (var i = 0; i < samps_per_symbol; i++)
            {
                var mark_p = (float)i * mark_inc;

                this.MarkI[i] = (float)Math.Sin(mark_p);
                this.MarkQ[i] = (float)Math.Cos(mark_p);

                var space_p = (float)i * space_inc;

                this.SpaceI[i] = (float)Math.Sin(space_p);
                this.SpaceQ[i] = (float)Math.Cos(space_p);
            }

            this.Buffer = new FixedSizeQueue<float>(samps_per_symbol);
        }

        public float[] Process(float value)
        {
            this.Buffer.Enqueue(value);

            // mark correlation
            var mark_product_s = 0f;
            var mark_product_c = 0f;

            for (var x = 0; x < this.Buffer.Count; x++)
            {
                var v = this.Buffer.Queue[x];
                var s = this.MarkI[x];
                var c = this.MarkQ[x];

                mark_product_s += v * s;
                mark_product_c += v * c;
            }

            mark_product_s = (float)Math.Pow(mark_product_s, 2);
            mark_product_c = (float)Math.Pow(mark_product_c, 2);

            // space correlation
            var space_product_s = 0f;
            var space_product_c = 0f;

            for (var x = 0; x < this.Buffer.Count; x++)
            {
                var v = this.Buffer.Queue[x];
                var s = this.SpaceI[x];
                var c = this.SpaceQ[x];

                space_product_s += v * s;
                space_product_c += v * c;
            }

            space_product_s = (float)Math.Pow(space_product_s, 2);
            space_product_c = (float)Math.Pow(space_product_c, 2);

            var results = new float[this.SpaceMultiplers.Length];

            var mark_mag = (float)Math.Sqrt(mark_product_s + mark_product_c);
            var space_mag = (float)Math.Sqrt(space_product_s + space_product_c);

            for (var i = 0; i < this.SpaceMultiplers.Length; i++)
            {
                results[i] = mark_mag - (space_mag * this.SpaceMultiplers[i]);
            }

            return results;
        }

        public float[,] Process(float[] values)
        {
            var results = new float[this.SpaceMultiplers.Length, values.Length];

            for (var i = 0; i < values.Length; i++)
            {
                var result = this.Process(values[i]);

                for (var y = 0; y < this.SpaceMultiplers.Length; y++)
                {
                    results[y, i] = result[y];
                }
            }

            return results;
        }
    }
}