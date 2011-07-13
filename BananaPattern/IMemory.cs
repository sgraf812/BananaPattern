using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BananaPattern
{
    public interface IMemory
    {
        byte[] ReadBytes(IntPtr begin, int count);

        T Read<T>(IntPtr address);
    }
}
