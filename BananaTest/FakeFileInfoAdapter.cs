using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using BananaXmlOffset;

namespace BananaTest
{
    class FakeFileInfoAdapter : IFileInfoAdapter
    {
        public Stream Stream { get; set; }
        private DateTime? _lastWriteTime;

        public FakeFileInfoAdapter()
        {
        }

        public FakeFileInfoAdapter(string value)
        {
            Stream = GetStringAsStream(value);
        }

        private Stream GetStringAsStream(string value)
        {
            MemoryStream ms = new MemoryStream();
            StreamWriter sr = new StreamWriter(ms);
            sr.Write(value);
            sr.Flush();
            return ms;
        }

        #region IFileInfoAdapter Member

        public DateTime LastWriteTime
        {
            get { return _lastWriteTime ?? DateTime.Now; }
            set { _lastWriteTime = value; }
        }

        public Stream OpenRead()
        {
            Stream.Position = 0;
            return Stream;
        }

        public Stream OpenWrite()
        {
            Stream.Position = 0;
            return Stream;
        }

        #endregion
    }
}
