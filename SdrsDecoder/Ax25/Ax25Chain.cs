using SdrsDecoder.Support;
using System;

namespace SdrsDecoder.Ax25
{
    public class Ax25Chain : ChainBase
    {
        private Interpolator interpolator;
        private ChebyFilter filter;
        private Decimator decimator;
        private DcRemover dcRemover;
        private IqDemod iqDemodulator;
        private ChebyFilter[] filter2;
        private Fsk2Demodulator[] fskDemodulator;
        private Unstuffer[] unstuffer;

        private NrzDecoder[] nrzDecoder;
        private Ax25Decoder[] ax25Decoder;

        //static float[] SpaceMultipliers = new float[] { 1.0f, 1.1f, 1.2f, 1.3f, 1.4f, 1.5f, 1.6f, 1.7f, 1.8f, 1.9f, 2.0f };
        //static float[] SpaceMultipliers = new float[] { 1.0f };

        static float[] SpaceMultipliers = new float[] { 1.0f };

        public ResampleValues Rv { get; private set; }

        private static T[] GetMultipliedObject<T>(Func<float, T> func)
        {
            var result = new T[SpaceMultipliers.Length];

            for (var i = 0; i < SpaceMultipliers.Length; i++)
            {
                result[i] = func(SpaceMultipliers[i]);
            }

            return result;
        }

        public Ax25Chain(float sampleRate, Action<MessageBase> messageReceived) : base(sampleRate, messageReceived)
        {
            var baud = 1200;
            var mark = 1200;
            var space = 2200;

            this.Rv = GetResampleValues(baud, sampleRate);

            var filterFactor = 1.2f;

            interpolator = new Interpolator(Rv.i);
            filter = new ChebyFilter(space * filterFactor, 1f, Rv.isr);
            decimator = new Decimator(Rv.d);
            dcRemover = new DcRemover(Rv.dsr, baud);
            iqDemodulator = new IqDemod(Rv.dsr, baud, mark, space, SpaceMultipliers);

            filter2 = GetMultipliedObject((float sm) => new ChebyFilter(baud * filterFactor, 1f, Rv.dsr));

            fskDemodulator = GetMultipliedObject((float sm) => { var pll = new Pll(Rv.dsr, baud); return new Fsk2Demodulator(baud, Rv.dsr, pll, false); });
            unstuffer = GetMultipliedObject((float sm) => { return new Unstuffer(); });
            nrzDecoder = GetMultipliedObject((float sm) => new NrzDecoder(0xff, 0x7e, NrzMode.Ax25));
            ax25Decoder = GetMultipliedObject((float sm) => new Ax25Decoder(messageReceived, sm));
        }

        public override void Process(float[] values, Action<float> writeSample = null)
        {
            var processed_values = interpolator.Process(values);
            processed_values = filter.Process(processed_values);
            processed_values = decimator.Process(processed_values);
            processed_values = dcRemover.Process(processed_values);

            var iqdemod_multiplied = iqDemodulator.Process(processed_values);

            for (var x = 0; x < SpaceMultipliers.Length; x++)
            {
                var iqdemod_values = new float[processed_values.Length];

                for (var y = 0; y < processed_values.Length; y++)
                {
                    iqdemod_values[y] = iqdemod_multiplied[x, y];
                }

                iqdemod_values = filter2[x].Process(iqdemod_values);

                var fsk_demodulated_values =
                    x == 0 ?
                    fskDemodulator[x].Process(iqdemod_values, writeSample) :
                    fskDemodulator[x].Process(iqdemod_values);

                foreach (var value in fsk_demodulated_values)
                {
                    var nrz_decode = nrzDecoder[x].Process(value);

                    var current_ax25Decoder = ax25Decoder[x];

                    if (nrz_decode.IsFlag || current_ax25Decoder.Frame.Bits.Count > (10 * 1024 * 8))
                    {
                        current_ax25Decoder.Flag();
                    }

                    var unstuffed = unstuffer[x].Process(nrz_decode.Value);

                    if (unstuffed.HasValue)
                    {
                        current_ax25Decoder.Process(unstuffed.Value);
                    }
                }
            }
        }
    }
}
