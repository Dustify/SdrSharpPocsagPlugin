using System;
using System.Collections.Generic;

namespace SdrsDecoder.Support
{
    public struct NrzResult
    {
        public bool Value { get; set; }
        public bool IsFlag { get; set; }
    }

    public enum NrzMode
    {
        Nrzi,
        Nrz // ? for acars anyway
    }

    public class NrzDecoder
    {
        private int CurrentValue = 0;
        private bool LastBit = true;
        private int flagMask;
        private int flag;
        private NrzMode mode;
        private bool LastValue = true;

        public NrzDecoder(int flagMask, int flag, NrzMode mode) {
            this.flagMask = flagMask;
            this.flag = flag;
            this.mode = mode;
        }

        public NrzResult Process(bool value)
        {
            var result = false;

            if (this.mode == NrzMode.Nrzi)
            {
                // no change means 1
                if (value == LastBit)
                {
                    result = true;
                }
            }

            if (this.mode == NrzMode.Nrz)
            {
                result = LastValue;

                // 0 means change
                if (value == false)
                {
                    result = !result;
                }

                LastValue = result;
            }

            LastBit = value;

            CurrentValue = CurrentValue << 1;

            if (result)
            {
                CurrentValue |= 1;
            }

            var isFlag = false;

            if ((CurrentValue & flagMask) == flag)
            {
                isFlag = true;
            }

            return new NrzResult
            {
                Value = result,
                IsFlag = isFlag
            };
        }

        //public bool[] Process(bool[] values)
        //{
        //    var results = new List<bool>();

        //    foreach (var value in values)
        //    {
        //        results.Add(this.Process(value));
        //    }

        //    return results.ToArray();
        //}
    }
}