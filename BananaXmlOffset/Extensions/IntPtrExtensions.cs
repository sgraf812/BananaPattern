using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BananaXmlOffset.Extensions
{
    static class IntPtrExtensions
    {
        public static unsafe int GetInt32OffsetFrom(this IntPtr to, IntPtr from)
        {
            byte* t = (byte*)to, f = (byte*)from;
            return (int)(t - f);
        }
    }
}
