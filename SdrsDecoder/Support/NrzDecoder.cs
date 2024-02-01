using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SdrsDecoder.Support
{
    public struct NrzResponse
    {
        public bool Value;
        public uint Decode;

        public bool HasValue;
    }

    public class NrzDecoder
    {
        private bool LastValue;

        private uint DecodeUnstuffed;

        private uint DecodeRaw;

        public NrzResponse Process(bool value)
        {
            var response = new NrzResponse()
            {
                HasValue = true
            };

            response.Value = this.LastValue == value;

            this.DecodeRaw = this.DecodeRaw << 1;

            if (response.Value)
            {
                this.DecodeRaw |= 1;
            }

            if ((this.DecodeRaw & 0x3f) == 0x3e)
            {
                response.HasValue = false;
            }
            else
            {
                this.DecodeUnstuffed = this.DecodeUnstuffed << 1;

                if (response.Value)
                {
                    this.DecodeUnstuffed |= 1;
                }

                response.Decode = this.DecodeUnstuffed;
            }

            this.LastValue = value;

            return response;
        }

        public NrzResponse[] Process(bool[] values)
        {
            var results = new List<NrzResponse>();

            foreach (var value in values)
            {
                results.Add(this.Process(value));
            }

            return results.ToArray();
        }
    }
}