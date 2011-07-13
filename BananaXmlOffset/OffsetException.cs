using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BananaXmlOffset
{
    [Serializable]
    public class OffsetException : Exception
    {
        public OffsetException() { }
        public OffsetException(string message) : base(message) { }
        public OffsetException(string message, Exception inner) : base(message, inner) { }
        protected OffsetException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
