using SdrsDecoder.Support;
using System;

namespace SdrsDecoder.Acars
{
    public class AcarsChain : ChainBase
    {
        public ResampleValues Rv;
        private Interpolator interpolator;
        private ChebyFilter filter;
        private Decimator decimator;
        private DcRemover dcRemover;
        private IqDemod iqDemodulator;
        private ChebyFilter[] filter2;
        private Fsk2Demodulator[] fskDemodulator;
        private NrzDecoder[] nrzDecoder;
        private AcarsDecoder[] acarsDecoders;

        static float[] SpaceMultipliers = new float[] { 1.0f };

        private static T[] GetMultipliedObject<T>(Func<float, T> func)
        {
            var result = new T[SpaceMultipliers.Length];

            for (var i = 0; i < SpaceMultipliers.Length; i++)
            {
                result[i] = func(SpaceMultipliers[i]);
            }

            return result;
        }

        public AcarsChain(float sampleRate, Action<MessageBase> messageReceived) : base(sampleRate, messageReceived)
        {
            var baud = 2400;

            var filterFactor = 1.2f;

            Rv = GetResampleValues(baud, sampleRate);
            interpolator = new Interpolator(Rv.i);
            filter = new ChebyFilter(baud * filterFactor, 1f, Rv.isr);
            decimator = new Decimator(Rv.d);
            dcRemover = new DcRemover(Rv.dsr, baud);

            iqDemodulator = new IqDemod(Rv.dsr, baud, 2400, 1200, SpaceMultipliers);
            filter2 = GetMultipliedObject((float sm) => new ChebyFilter(baud * filterFactor, 1f, Rv.dsr));
            fskDemodulator = GetMultipliedObject((float sm) => { var pll = new Pll(Rv.dsr, baud); return new Fsk2Demodulator(baud, Rv.dsr, pll, false); });
            nrzDecoder = GetMultipliedObject((float sm) => new NrzDecoder(0xFFFFFF, 0x686880, NrzMode.Acars));
            acarsDecoders = GetMultipliedObject((float sm) => new AcarsDecoder(messageReceived));
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

                    var acarsDecoder = acarsDecoders[x];

                    if (nrz_decode.IsFlag || acarsDecoder.Bits.Count > (10 * 1024 * 8))
                    {
                        acarsDecoder.Flag();
                    }

                    if (!nrz_decode.IsFlag)
                    {
                        acarsDecoder.Process(nrz_decode.Value);
                    }
                }
            }
        }
    }
}
