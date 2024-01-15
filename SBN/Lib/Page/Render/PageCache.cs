using Microsoft.Extensions.Caching.Memory;
using System;

namespace SBN.Lib.Page.Render
{
    public class PageCache : IPageCache
    {
        private IMemoryCache _cachedPages;

        public PageCache(IMemoryCache cachedPages)
        {
            _cachedPages = cachedPages;
        }

        public string Get(string pathAndQuery)
        {
            var exists = _cachedPages.TryGetValue<string>(pathAndQuery, out var body);

            return exists
                ? body
                : null; 
        }

        public void Set(string pathAndQuery, string pageTemplateString, DateTime cacheExpiry)
        {
            _cachedPages.Set(pathAndQuery, pageTemplateString, cacheExpiry);
        }
    }
}
