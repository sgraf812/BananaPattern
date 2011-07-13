using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BananaPattern.Algorithms
{
    /// <summary>
    /// Naive implementation of a pattern matching algorithm.
    /// </summary>
    public class NaiveAlgorithm : IPatternAlgorithm
    {
        /// <summary>
        /// The stateless instance of this algorithm.
        /// </summary>
        public static readonly NaiveAlgorithm Instance = new NaiveAlgorithm();

        private NaiveAlgorithm() { }

        public unsafe IntPtr Apply(byte[] pattern, bool[] mask, byte* begin, byte* end)
        {
            end -= pattern.Length - 1;

            bool matches;
            for (; begin < end; begin++)
            {
                matches = true;
                for (int i = 0; i < pattern.Length; i++)
                {
                    if (mask[i] && pattern[i] != begin[i])
                    {
                        matches = false;
                        break;
                    }
                }

                if (matches)
                {
                    return (IntPtr)begin;
                }
            }

            return IntPtr.Zero;
        }
    }
}
