using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace BananaXmlOffset
{
    public abstract class CachedNamedOffsetProvider : INamedOffsetProvider
    {
        private readonly IDictionary<string, IntPtr> _addressCache = new ConcurrentDictionary<string, IntPtr>();

        #region INamedOffsetProvider Member

        public virtual bool CanResolve(string name)
        {
            return _addressCache.ContainsKey(name);
        }

        public virtual IntPtr GetAddress(string name)
        {
            IntPtr address;
            if (_addressCache.TryGetValue(name, out address))
            {
                return address;
            }

            bool addResultToCache;
            address = CalculateAddress(name, out addResultToCache);

            if (addResultToCache)
            {
                _addressCache[name] = address;
            }

            return address;
        }

        #endregion

        protected abstract IntPtr CalculateAddress(string name, out bool addResultToCache);
    }
}
