using System;
using System.Numerics;

namespace SdrsDecoder.Support
{
    public struct ResampleValues
    {
        public int i { get; set; }
        public int d { get; set; }
        public int isr { get; set; }
        public int dsr { get; set; }
    }

    public abstract class ChainBase
    {
        protected float sampleRate;
        protected Action<MessageBase> messageReceived;

        public ChainBase(float sampleRate, Action<MessageBase> messageReceived)
        {
            this.sampleRate = sampleRate;
            this.messageReceived = messageReceived;
        }

        public abstract void Process(float[] values, Action<float> writeSample = null);

        public static ResampleValues GetResampleValues(float baud, float sampleRate)
        {
            var targetRate = (int)baud * 10;
            var gcd = (int)BigInteger.GreatestCommonDivisor((BigInteger)sampleRate, (BigInteger)targetRate);

            var result = new ResampleValues();

            result.i = targetRate / gcd;
            result.d = (int)sampleRate / gcd;

            if (result.i > 100)
            {
                result.i = 1;
                result.d = 1;
            }

            result.isr = (int)sampleRate * result.i;
            result.dsr = result.isr / result.d;

            return result;
        }
    }
}
