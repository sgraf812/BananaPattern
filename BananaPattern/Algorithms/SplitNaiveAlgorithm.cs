using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BananaPattern.Algorithms
{
    public class SplitNaiveAlgorithm : IPatternAlgorithm
    {
        /// <summary>
        /// The stateless instance of this Algorithm.
        /// </summary>
        public static readonly SplitNaiveAlgorithm Instance = new SplitNaiveAlgorithm();

        private SplitNaiveAlgorithm()
        {
        }

        #region IPatternAlgorithm Member

        public unsafe IntPtr Apply(byte[] pattern, bool[] mask, byte* begin, byte* end)
        {
            // split pattern at wildcards
            var ranges = GetMatchRanges(pattern, mask);

            end -= pattern.Length - 1;

            bool matches;
            for (; begin < end; begin++)
            {
                matches = true;
                for (int r = 0; r < ranges.Count && matches; r++)
                {
                    var range = ranges[r];
                    var rangeStart = range.Start;
                    var rangeEnd = range.Start + range.Length;

                    for (int i = rangeStart; i < rangeEnd && matches; i++)
                    {
                        if (pattern[i] != begin[i])
                        {
                            matches = false;
                        }
                    }
                }

                if (matches)
                {
                    return (IntPtr)begin;
                }
            }

            return IntPtr.Zero;
        }

        #endregion

        private IList<PatternRange> GetMatchRanges(byte[] pattern, bool[] mask)
        {
            List<PatternRange> ranges = new List<PatternRange>();

            for (int i = 0; i < pattern.Length;)
            {
                if (mask[i])
                {
                    var patternRange = new PatternRange
                    {
                        Start = i,
                        Length = pattern
                                    .Skip(i)
                                    .TakeWhile((b, idx) => mask[i + idx])
                                    .Count()
                    };

                    ranges.Add(patternRange);

                    i += patternRange.Length + 1; // next byte will be a wildcard, so advance one more
                }
                else
                {
                    i++;
                }
            }

            return ranges;
        }

        private struct PatternRange
        {
            public int Start { get; set; }
            public int Length { get; set; }
        }
    }
}
