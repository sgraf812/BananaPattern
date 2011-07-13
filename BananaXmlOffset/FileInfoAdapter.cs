using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BananaXmlOffset
{
    public class FileInfoAdapter : IFileInfoAdapter
    {
        private FileInfo _fileInfo;

        public FileInfoAdapter(string fileName)
        {
            _fileInfo = new FileInfo(fileName);
        }

        #region IFileInfoAdapter Member

        public DateTime LastWriteTime
        {
            get { return _fileInfo.LastWriteTime; }
        }

        public System.IO.Stream OpenRead()
        {
            return _fileInfo.OpenRead();
        }

        public System.IO.Stream OpenWrite()
        {
            return _fileInfo.OpenWrite();
        }

        #endregion
    }
}
