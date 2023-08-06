using System;

namespace SdrsDecoder.Support
{
    struct BchResult
    {
        public uint Value;
        public bool BchErrors;
        public bool ParityError;
        public int Corrected;
    }

    internal class Bch
    {
        public const uint Generator = 1897;

        public static bool CheckBchError(uint codeWordAsUInt)
        {
            uint remainder = codeWordAsUInt >> 1;

            for (var i = 30; i >= 0; i--)
            {
                if ((remainder & (1u << i)) != 0) // Is the bit present at index i?
                {
                    for (var x = 0; x < 11; x++)
                    {
                        var position = i - x;
                        var generatorValue = (Generator & (1 << (10 - x))) > 0;

                        // XOR the current remainder bit with the generator bit
                        if ((remainder & (1u << position)) > 0 ^ generatorValue)
                        {
                            remainder |= (uint)1 << position;
                        }
                        else
                        {
                            remainder &= ~((uint)1 << position);
                        }
                    }
                }
            }

            return remainder > 0;
        }

        public static BchResult Process(uint value)
        {
            var bchErrors = false;
            var errorsCorrected = 0;

            var valueResult = value;

            if (CheckBchError(valueResult))
            {
                bchErrors = true;

                // 1 bit error correction
                for (var i = 1; i < 32 && bchErrors; i++)
                {
                    var valueToCheck = value ^ (1U << i);

                    if (!CheckBchError(valueToCheck))
                    {
                        bchErrors = false;
                        errorsCorrected++;
                        valueResult = valueToCheck;
                    }
                }

                // 2 bit error correction
                if (bchErrors)
                {
                    for (var x = 1; x < 32 && bchErrors; x++)
                    {
                        for (var y = 1; y < 32 && bchErrors; y++)
                        {
                            if (x == y)
                            {
                                continue;
                            }

                            var valueToCheck = value ^ (1U << x);
                            valueToCheck = valueToCheck ^ (1U << y);

                            if (!CheckBchError(valueToCheck))
                            {
                                bchErrors = false;
                                errorsCorrected += 2;
                                valueResult = valueToCheck;
                            }
                        }
                    }
                }
            }

            var parityCount = 0;

            for (var i = 1; i < 32; i++)
            {
                if ((valueResult & (1U << i)) != 0)
                {
                    parityCount++;
                }
            }

            var calculatedParityBit = (parityCount % 2) == 0 ? 0U : 1U;
            var existingParityBit = (valueResult >> 0) & 1U;

            var parityError = calculatedParityBit != existingParityBit;

            return new BchResult
            {
                Corrected = errorsCorrected,
                BchErrors = bchErrors,
                ParityError = parityError,
                Value = valueResult
            };
        }
    }
}

