using System;
using System.Collections.Generic;

namespace SdrsDecoder.Support
{
    public struct NrzResult
    {
        public bool Value { get; set; }
        public bool IsFlag { get; set; }
    }

    public class NrzDecoder
    {
        private int CurrentValue = 0;
        private bool LastBit = true;

        public NrzResult Process(bool value)
        {
            var result = false;

            // no change means 1
            if (value == LastBit)
            {
                result = true;
            }

            LastBit = value;

            CurrentValue = CurrentValue << 1;

            if (result)
            {
                CurrentValue |= 1;
            }

            var isFlag = false;

            if ((byte)(CurrentValue & 0xFF) == 0x7e)
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