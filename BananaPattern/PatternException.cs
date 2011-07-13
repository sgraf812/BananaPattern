using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BananaPattern
{
    /// <summary>
    /// Thrown when an error occurs while working with the Pattern class.
    /// </summary>
    [Serializable]
    public class PatternException : Exception
    {
        public PatternException() { }
        public PatternException(string message) : base(message) { }
        public PatternException(string message, Exception inner) : base(message, inner) { }
        protected PatternException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

}
