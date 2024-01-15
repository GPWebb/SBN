using Microsoft.Extensions.Caching.Memory;
using System;
using System.Xml.Linq;

namespace SBN.Lib.Action.Data
{
    public class ActionDataCache : IActionDataCache
    {
        private IMemoryCache _actionDataCache;

        public ActionDataCache(IMemoryCache actionDataCache)
        {
            _actionDataCache = actionDataCache;
        }

        public void Set(string cacheKey, XElement apiData, DateTimeOffset expiryDateTime)
        {
            _actionDataCache.Set(cacheKey, apiData, expiryDateTime);
        }

        public bool TryGet(string cacheKey, out XElement apiData)
        {
            return _actionDataCache.TryGetValue(cacheKey, out apiData);
        }
    }
}
