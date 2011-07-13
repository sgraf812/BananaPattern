using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BananaPattern.Algorithms
{
    /// <summary>
    /// Implement this interface for use with the Pattern class.
    /// </summary>
    public interface IPatternAlgorithm
    {
        /// <summary>
        /// Applies the pattern matching algorithm to the specified address range.
        /// </summary>
        /// <param name="pattern">Byte pattern to match.</param>
        /// <param name="mask">Matching mask.</param>
        /// <param name="begin">Begin of address range.</param>
        /// <param name="end">End of address range.</param>
        /// <returns>The address of the match or <c>IntPtr.Zero</c> if the pattern could not be matched.</returns>
        unsafe IntPtr Apply(byte[] pattern, bool[] mask, byte* begin, byte* end);
    }
}
