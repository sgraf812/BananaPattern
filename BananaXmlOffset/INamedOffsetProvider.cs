using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BananaXmlOffset
{
    public interface INamedOffsetProvider
    {
        bool CanResolve(string name);
        IntPtr GetAddress(string name);
    }
}
