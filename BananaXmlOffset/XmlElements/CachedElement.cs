using System;
using System.Diagnostics;
using System.Xml.Linq;
using BananaXmlOffset.Extensions;

namespace BananaXmlOffset.XmlElements
{
    internal class CachedElement : XElementWrapper
    {
        public virtual int Value
        {
            get
            {
                return Convert.ToInt32(GetAttributeValue("Value"), 0x10);
            }
            set
            {
                SetAttribute("Value", value.ToString("X"));
            }
        }
        public virtual string Build
        {
            get
            {
                return GetAttributeValue("Build").Trim();
            }
            set
            {
                SetAttribute("Build", value);
            }
        }

        public CachedElement()
            : base(new XElement("Cached",
                new XAttribute("Build", ""),
                new XAttribute("Value", "")))
        {
        }

        public CachedElement(XElement element)
            : base(element)
        {
        }

        public virtual bool IsSameVersion(FileVersionInfo info)
        {
            return Build == info.FileVersion;
        }

        public virtual void Update(IntPtr value, ProcessModule targetModule)
        {
            IntPtr baseAddress = targetModule.BaseAddress;
            FileVersionInfo info = targetModule.FileVersionInfo;

            int offset = value.GetInt32OffsetFrom(targetModule.BaseAddress);
            Build = info.FileVersion;
            Value = offset;
        }

        public virtual bool TryGetCachedValue(ProcessModule targetModule, out IntPtr result)
        {
            result = IntPtr.Zero;
            IntPtr baseAddress = targetModule.BaseAddress;
            FileVersionInfo info = targetModule.FileVersionInfo;

            if (IsSameVersion(info))
            {
                result += Value;
                return true;
            }

            return false;
        }
    }
}
