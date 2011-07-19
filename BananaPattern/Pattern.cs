using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BananaPattern.Algorithms;
using BananaPattern.Extensions;
using System.Diagnostics;

namespace BananaPattern
{
    /// <summary>
    /// Pattern class for finding byte sequence matches in memory.
    /// </summary>
    public class Pattern
    {
        /// <summary>
        /// Default pattern matching algorithm applied when Find() and FindMany() are passed null as algorithm.
        /// </summary>
        public static IPatternAlgorithm DefaultAlgorithm { get; set; }

        internal byte[] _pattern;

        internal bool[] _mask;

        /// <summary>
        /// Initializes the static part of the Pattern class.
        /// </summary>
        static Pattern()
        {
            ResetInternals();
        }

        internal static void ResetInternals()
        {
            DefaultAlgorithm = BoyerMooreHorspoolAlgorithm.Instance;
        }

        /// <summary>
        /// Constructs a new Pattern instance.
        /// </summary>
        /// <param name="pattern">Byte sequence to match.</param>
        /// <param name="mask">Boolean mask specifying which elements of <paramref name="pattern"/> must match.</param>
        /// <param name="trimWildcards">If true, the wildcard elements will be trimmed.</param>
        public Pattern(byte[] pattern, bool[] mask = null, bool trimWildcards = true)
        {
            _pattern = pattern;
            _mask = mask;

            if (_mask == null)
            {
                _mask = new bool[pattern.Length];
                for (int i = 0; i < _mask.Length; i++)
                {
                    _mask[i] = true;
                }
            }
            else if (trimWildcards && _mask.Length > 0)
            {
                int first = _mask.FindFirst(b => b);
                int last = _mask.FindLast(b => b);

                TrimPatternAndMask(first, last);
            }

            Debug.Assert(_pattern.Length == _mask.Length, "Pattern and mask have to be of the same length.");
        }

        private void TrimPatternAndMask(int first, int last)
        {
            int length = last - first + 1;
            if (length != _mask.Length)
            {
                byte[] newPattern = new byte[length];
                bool[] newMask = new bool[length];

                for (int i = 0; i < length; i++)
                {
                    newPattern[i] = _pattern[first + i];
                    newMask[i] = _mask[first + i];
                }

                _pattern = newPattern;
                _mask = newMask;
            }
        }

        /// <summary>
        /// Initializes a new Pattern instance from the specified combined pattern string.
        /// </summary>
        /// <param name="pattern">The pattern string containing seperated byte values.</param>
        /// <param name="seperator">Optional seperator sequence byte values.</param>
        /// <param name="wildcard">Optional wildcard sequence. Must contain at least one non-digit character.</param>
        /// <param name="fromBase">Optional numeric base to convert from.</param>
        /// <returns>A prepared Pattern instance.</returns>
        public static Pattern FromCombinedString(string pattern, string seperator = " ", string wildcard = "?", int fromBase = 0x10)
        {
            Debug.Assert(wildcard.Any(c => !char.IsDigit(c)), 
                "Wildcards have to contain at least one non digit character.");

            string[] values = pattern.Split(new string[] { seperator }, StringSplitOptions.RemoveEmptyEntries);

            byte[] bytes = new byte[values.Length];
            bool[] mask = new bool[values.Length];

            for (int i = 0; i < values.Length; i++)
            {
                string current = values[i];

                if (current.Contains(wildcard))
                {
                    mask[i] = false;
                }
                else
                {
                    mask[i] = true;
                    bytes[i] = Convert.ToByte(values[i], fromBase);
                }
            }

            return new Pattern(bytes, mask);
        }

        /// <summary>
        /// Formats the patterns internals into a combined pattern string.
        /// </summary>
        /// <param name="seperator">Optional seperator sequence byte values.</param>
        /// <param name="wildcard">Optional wildcard sequence. Must contain at least one non-digit character.</param>
        /// <param name="toBase">Optional numeric base to convert to.</param>
        /// <returns></returns>
        public string ToCombinedString(string seperator = " ", string wildcard = "?", int toBase = 0x10)
        {
            // wildcards should contain at least one non digit character.
            Debug.Assert(wildcard.Where(c => !char.IsDigit(c)).Any());

            var values = _pattern.Select((b, i) =>
            {
                if (_mask[i])
                    return Convert.ToString(b, toBase).ToUpper();
                else
                    return wildcard;
            });

            return string.Join(seperator, values);
        }

        /// <summary>
        /// Find a byte pattern in the MainModule of <paramref name="context"/>'s TargetProcess.
        /// </summary>
        /// <param name="context">ProcessContext for the target process.</param>
        /// <param name="algorithm">An algorithm object which applies the pattern. The default is null, so that the DefaultAlgorithm is used.</param>
        /// <param name="maxMatchCount">Maximum number of matches to return.</param>
        /// <returns>A list of the search results.</returns>
        /// <remarks>Does not throw in exception in case of no match is found.</remarks>
        public virtual IList<IntPtr> FindMany(IBotProcessContext context, int maxMatchCount, IPatternAlgorithm algorithm = null)
        {
            return FindMany(context.TargetProcess.MainModule, context, maxMatchCount, algorithm);
        }

        /// <summary>
        /// Find a byte pattern in the ProcessModule <paramref name="module"/>.
        /// </summary>
        /// <param name="module">The ProcessModule.</param>
        /// <param name="context">ProcessContext for the target process.</param>
        /// <param name="algorithm">An algorithm object which applies the pattern. The default is null, so that the DefaultAlgorithm is used.</param>
        /// <param name="maxMatchCount">Maximum number of matches to return.</param>
        /// <returns>A list of the search results.</returns>
        /// <remarks>Does not throw in exception in case of no match is found.</remarks>
        public virtual IList<IntPtr> FindMany(ProcessModule module, IBotProcessContext context, int maxMatchCount, IPatternAlgorithm algorithm = null)
        {
            return FindMany(module.BaseAddress, module.BaseAddress + module.ModuleMemorySize, context, maxMatchCount, algorithm);
        }

        /// <summary>
        /// Find a byte pattern in the memory range from <paramref name="begin"/> to <paramref name="end"/>.
        /// </summary>
        /// <param name="begin">Start address for searching.</param>
        /// <param name="end">One after the last address for searching.</param>
        /// <param name="context">ProcessContext for the target process.</param>
        /// <param name="algorithm">An algorithm object which applies the pattern. The default is null, so that the DefaultAlgorithm is used.</param>
        /// <param name="maxMatchCount">Maximum number of matches to return.</param>
        /// <returns>A list of the search results.</returns>
        /// <remarks>Does not throw in exception in case of no match is found.</remarks>
        public virtual IList<IntPtr> FindMany(IntPtr begin, IntPtr end, IBotProcessContext context, int maxMatchCount, IPatternAlgorithm algorithm = null)
        {
            return FindImpl(begin, end, context, algorithm, maxMatchCount);
        }

        /// <summary>
        /// Find a byte pattern in the MainModule of <paramref name="context"/>'s TargetProcess.
        /// </summary>
        /// <param name="context">ProcessContext for the target process.</param>
        /// <param name="algorithm">An algorithm object which applies the pattern. The default is null, so that the DefaultAlgorithm is used.</param>
        /// <returns>The first pattern match in the specified address range.</returns>
        /// <exception cref="PatternException">Pattern could not be matched.</exception>
        public virtual IntPtr Find(IBotProcessContext context, IPatternAlgorithm algorithm = null)
        {
            return Find(context.TargetProcess.MainModule, context, algorithm);
        }

        /// <summary>
        /// Find a byte pattern in the ProcessModule <paramref name="module"/>.
        /// </summary>
        /// <param name="module">The ProcessModule.</param>
        /// <param name="end">One after the last address for searching.</param>
        /// <param name="context">ProcessContext for the target process.</param>
        /// <param name="algorithm">An algorithm object which applies the pattern. The default is null, so that the DefaultAlgorithm is used.</param>
        /// <returns>The first pattern match in the specified address range.</returns>
        /// <exception cref="PatternException">Pattern could not be matched.</exception>
        public virtual IntPtr Find(ProcessModule module, IBotProcessContext context, IPatternAlgorithm algorithm = null)
        {
            return Find(module.BaseAddress, module.BaseAddress + module.ModuleMemorySize, context, algorithm);
        }

        /// <summary>
        /// Find a byte pattern in the memory range from <paramref name="begin"/> to <paramref name="end"/>.
        /// </summary>
        /// <param name="begin">Start address for searching.</param>
        /// <param name="end">One after the last address for searching.</param>
        /// <param name="context">ProcessContext for the target process.</param>
        /// <param name="algorithm">An algorithm object which applies the pattern. The default is null, so that the DefaultAlgorithm is used.</param>
        /// <returns>The first pattern match in the specified address range.</returns>
        /// <exception cref="PatternException">Pattern could not be matched.</exception>
        public virtual IntPtr Find(IntPtr begin, IntPtr end, IBotProcessContext context, IPatternAlgorithm algorithm = null)
        {
            var list = FindImpl(begin, end, context, algorithm, 1);

            if (list.Count == 0)
            {
                throw new PatternException("Pattern could not be matched.");
            }

            return list[0];
        }

        private unsafe IList<IntPtr> FindImpl(IntPtr begin, IntPtr end, IBotProcessContext context, IPatternAlgorithm algorithm, int maxMatchCount)
        {
            algorithm = algorithm ?? DefaultAlgorithm;

            // if we are injected, simply forward the begin and end addresses.
            // otherwise, just copy the whole address range.
            if (context.IsInProcess)
            {
                return FindInProcess(begin, end, maxMatchCount, algorithm);
            }
            else
            {
                return FindOutOfProcess(begin, end, context, maxMatchCount, algorithm);
            }
        }

        private unsafe IList<IntPtr> FindInProcess(IntPtr begin, IntPtr end, int maxMatchCount, IPatternAlgorithm algorithm)
        {
            byte* b = (byte*)begin;
            byte* e = (byte*)end;

            IList<IntPtr> matches = new List<IntPtr>(maxMatchCount);

            while (matches.Count < maxMatchCount && b < e)
            {
                IntPtr match = algorithm.Apply(_pattern, _mask, b, e);

                if (match == IntPtr.Zero)
                {
                    break;
                }

                matches.Add(match);
                b = (byte*)match + 1;
            }

            return matches;
        }

        private unsafe IList<IntPtr> FindOutOfProcess(IntPtr begin, IntPtr end, IBotProcessContext context, int maxMatchCount, IPatternAlgorithm algorithm)
        {
            byte* b = (byte*)begin;
            byte* e = (byte*)end;

            IList<IntPtr> matches = new List<IntPtr>(maxMatchCount);

            int byteCount = (int)(e - b);

            byte[] buffer = context.Memory.ReadBytes(begin, byteCount);

            fixed (byte* buf = buffer)
            {
                byte* bufEnd = buf + buffer.Length;
                byte* cur = buf;

                while (matches.Count < maxMatchCount && cur < bufEnd)
                {
                    IntPtr match = algorithm.Apply(_pattern, _mask, cur, bufEnd);

                    if (match == IntPtr.Zero)
                    {
                        break;
                    }

                    int delta = (int)((byte*)match - buf);
                    matches.Add(begin + delta);

                    cur = (byte*)match + 1;
                }
            }

            return matches;
        }
    }
}
