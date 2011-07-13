using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace BananaPattern
{
    public interface IBotProcessContext : IDisposable
    {
        bool IsInProcess { get; }
        Process TargetProcess { get; }
        IMemory Memory { get; }
    }
}
