using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdrsDecoder.Support.mathnet
{
    public enum ImpulseResponse
    {
        /// <summary>
        /// Impulse response always has a finite length of time and are stable, but usually have a long delay.
        /// </summary>
        Finite,

        /// <summary>
        /// Impulse response may have an infinite length of time and may be unstable, but usually have only a short delay.
        /// </summary>
        Infinite
    }
}
