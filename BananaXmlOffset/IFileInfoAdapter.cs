using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BananaXmlOffset
{
    public interface IFileInfoAdapter
    {
        DateTime LastWriteTime { get; }

        Stream OpenRead();
        Stream OpenWrite();
    }
}
