using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using BananaPattern;
using BananaPattern.Algorithms;

namespace BananaXmlOffset.XmlElements
{
    internal class PatternElement : XElementWrapper
    {
        public virtual CachedElement Cached
        {
            get
            {
                XElement cachedElement = Element.Elements("Cached").FirstOrDefault();
                return cachedElement != null ? new CachedElement(cachedElement) : null;
            }
            set
            {
                XElement cachedElement = Element.Elements("Cached").FirstOrDefault();
                if (cachedElement != null)
                {
                    if (value != null)
                    {
                        cachedElement.ReplaceWith(value.Element);
                    }
                    else
                    {
                        cachedElement.Remove();
                    }
                }
                else
                {
                    Element.AddFirst(value.Element);
                }
            }
        }
        public virtual string Name
        {
            get
            {
                return GetElementValue("Name").Trim();
            }
            set
            {
                SetElement("Name", value);
            }
        }
        public virtual Pattern Pattern
        {
            get
            {
                return Pattern.FromCombinedString(GetElementValue("Pattern").Replace('\n', ' '));
            }
            set
            {
                SetElement("Pattern", value.ToCombinedString());
            }
        }

        public PatternElement()
            : base(new XElement("Pattern",
                new XElement("Name", ""),
                new XElement("Pattern", "")))
        {
        }

        public PatternElement(XElement element)
            : base(element)
        {
        }

        public virtual IntPtr FindCached(IBotProcessContext context)
        {
            var mainModule = context.TargetProcess.MainModule;
            IntPtr result;
            if (NoCachedElement) Cached = new CachedElement();
            if (IsCachedElementOutdated(mainModule, out result))
            {
                result = Find(context);
                Cached.Update(result, mainModule);
            }
            return result;
        }

        private bool NoCachedElement
        {
            get
            {
                return Cached == null;
            }
        }

        private bool IsCachedElementOutdated(System.Diagnostics.ProcessModule mainModule, out IntPtr result)
        {
            return !Cached.TryGetCachedValue(mainModule, out result);
        }

        public virtual IntPtr Find(IBotProcessContext context)
        {
            return Pattern.Find(context);
        }
    }
}
