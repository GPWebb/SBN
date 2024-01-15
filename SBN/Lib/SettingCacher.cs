using System;
using Microsoft.Extensions.Caching.Memory;

namespace SBN.Lib
{
    public class SettingCacher : ISettingCacher
    {
        private IMemoryCache _settingCache;

        public SettingCacher(IMemoryCache settingCache)
        {
            _settingCache = settingCache;
        }

        public void Write(Guid sessionToken, string setting, string value)
        {
            _settingCache.Set($"{sessionToken}:{setting}", value, TimeSpan.FromMinutes(1)); //Should be refreshed per request so doesn't need to be kept for any length of time
        }

        public bool TryRead(Guid sessionToken, string setting, out string value)
        {
            return _settingCache.TryGetValue($"{sessionToken}:{setting}", out value);
        }

        public string Read(Guid sessionToken, string setting)
        {
            string value;
            if (_settingCache.TryGetValue($"{sessionToken}:{setting}", out value)) return value;

            throw new Exception($"Setting '{setting}' not available");
        }
    }
}
