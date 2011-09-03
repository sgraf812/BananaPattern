using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BananaPattern.Algorithms
{
    /// <summary>
    /// Implementation of the boyer-moore pattern matching algorithm.
    /// </summary>
    public class BoyerMooreHorspoolAlgorithm : IPatternAlgorithm
    {
        /// <summary>
        /// The stateless instance of this algorithm.
        /// </summary>
        public static readonly BoyerMooreHorspoolAlgorithm Instance = new BoyerMooreHorspoolAlgorithm();

        private BoyerMooreHorspoolAlgorithm() { }

        public unsafe IntPtr Apply(byte[] pattern, bool[] mask, byte* begin, byte* end)
        {
            // skip table for bad character rule, is fixed size
            int* skipTable = stackalloc int[byte.MaxValue + 1];

            // initialize with default shift value
            // -> sadly, index of first not matched value.
            int length = pattern.Length;
            int last = length - 1;
            int init = length;
            for (int i = last - 1; i >= 0; i--)
            {
                if (!mask[i])
	            {
                    init = last - i;
                    break;
	            }
            }

            for (int v = 0; v < byte.MaxValue + 1; v++)
            {
                skipTable[v] = init;
            }

            // i < last, because if a mismatch occurs at a character which only occurs at the 
            // last byte in the pattern, the shift will be 0 -> infinite loop!
            for (int i = 0; i < last; i++)
            {
                byte v = pattern[i];
                int newSkip = last - i;
                int oldSkip = skipTable[v];
                if (newSkip < oldSkip)
                {
                    skipTable[v] = newSkip;
                }
            }

            end -= last;
            while (begin < end)
            {
                for (int i = last; i >= 0; i--)
                {
                    if (mask[i] && begin[i] != pattern[i])
                    {
                        break;
                    }

                    // found it
                    if (i == 0)
                    {
                        return (IntPtr)begin;
                    }
                }

                begin += skipTable[begin[last]];
            }

            return IntPtr.Zero;
        }
    }
}