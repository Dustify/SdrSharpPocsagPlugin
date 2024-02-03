using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace SdrsDecoder.Support.mathnet
{
    public class OnlineIirFilter : OnlineFilter
    {
        readonly float[] _b;
        readonly float[] _a;
        readonly float[] _bufferX;
        readonly float[] _bufferY;
        readonly int _halfSize;
        int _offset;

        /// <summary>
        /// Infinite Impulse Response (IIR) Filter.
        /// </summary>
        public OnlineIirFilter(float[] coefficients)
        {
            if (null == coefficients)
            {
                throw new ArgumentNullException(nameof(coefficients));
            }

            if ((coefficients.Length & 1) != 0)
            {
                throw new ArgumentException("Number of coefs must be even", nameof(coefficients));
            }

            int size = coefficients.Length;
            _halfSize = size >> 1;
            _b = new float[size];
            _a = new float[size];

            for (int i = 0; i < _halfSize; i++)
            {
                _b[i] = _b[_halfSize + i] = coefficients[i];
                _a[i] = _a[_halfSize + i] = coefficients[_halfSize + i];
            }

            _bufferX = new float[size];
            _bufferY = new float[size];
        }

        /// <summary>
        /// Process a single sample.
        /// </summary>
        public override float ProcessSample(float sample)
        {
            _offset = (_offset != 0) ? _offset - 1 : _halfSize - 1;
            _bufferX[_offset] = sample;
            _bufferY[_offset] = 0f;
            float yn = 0f;

            for (int i = 0, j = _halfSize - _offset; i < _halfSize; i++, j++)
            {
                yn += _bufferX[i] * _b[j];
            }

            for (int i = 0, j = _halfSize - _offset; i < _halfSize; i++, j++)
            {
                yn -= _bufferY[i] * _a[j];
            }

            _bufferY[_offset] = yn;
            return yn;
        }

        /// <summary>
        /// Reset internal state (not coefficients!).
        /// </summary>
        public override void Reset()
        {
            for (int i = 0; i < _bufferX.Length; i++)
            {
                _bufferX[i] = 0f;
                _bufferY[i] = 0f;
            }
        }
    }
}
