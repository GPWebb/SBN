using Microsoft.Extensions.Caching.Memory;
using SBN.Lib.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SBN.Lib.Page.Render.Util
{
    public class SiteTextCache : ISiteTextCache
    {
        private IMemoryCache _cachedSiteText;
        private readonly ISiteText _siteTextReader;
        private readonly ISessionTokenAccessor _sessionTokenAccessor;

        public SiteTextCache(IMemoryCache memoryCache,
            ISiteText siteTextReader,
            ISessionTokenAccessor sessionTokenAccessor)
        {
            _cachedSiteText = memoryCache;
            _siteTextReader = siteTextReader;
            _sessionTokenAccessor = sessionTokenAccessor;
        }

        public async Task<IDictionary<string, string>> Read(IEnumerable<string> parameters)
        {
            var results = new Dictionary<string, string>();

            try
            {
                if (parameters.Any())
                {
                    foreach (var parameter in parameters.Distinct())
                    {
                        try
                        {
                            if (_cachedSiteText.TryGetValue(parameter, out string value))
                            {
                                results.Add(parameter, value);
                            }
                            else
                            {
                                results.Add(parameter, null);
                            }
                        }
                        catch (Exception ex)
                        {
                            //Swallow, we don't want a bad parameter add to somehow kill the lot
                            _ = ex; //Swallow the warning, ex is only declared for debugging
                        }
                    }

                    var dbText = await _siteTextReader.Read(_sessionTokenAccessor.SessionToken(), parameters);

                    foreach (var text in dbText)
                    {
                        _cachedSiteText.Set(text.Key, text.Value, TimeSpan.FromSeconds(30));
                        results[text.Key] = text.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                //Swallow for the moment 
                _ = ex; //Swallow the warning again
            }

            return results;
        }
    }
}
