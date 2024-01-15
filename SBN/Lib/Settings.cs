using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace SBN.Lib
{
    public class Settings : ISettings
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DB.ISettings _settings;
        private readonly ISessionTokenAccessor _sessionTokenAccessor;
        private readonly ISettingCacher _settingCacher;

        public Settings(IHttpContextAccessor httpContextAccessor,
            DB.ISettings settings,
            ISessionTokenAccessor sessionTokenAccessor,
            ISettingCacher settingCacher)
        {
            _httpContextAccessor = httpContextAccessor;
            _settings = settings;
            _sessionTokenAccessor = sessionTokenAccessor;
            _settingCacher = settingCacher;
        }

        public void CacheSetting(string setting, string value)
        {
            _settingCacher.Write(_sessionTokenAccessor.SessionToken(), setting, value);
        }

        public async Task<string> GetSetting(string setting)
        {
            switch (setting.Trim().ToUpper())
            {
                case "URL":
                    try
                    {
                        return _httpContextAccessor.HttpContext.Request.GetDisplayUrl();
                    }
                    catch
                    {
                        return "";
                    }

                case "REFERRER":
                    try
                    {
                        return _httpContextAccessor.HttpContext.Request.Headers["Referer"].ToString();
                    }
                    catch
                    {
                        return "";
                    }

                case "UTCDATE":
                    return DateTime.UtcNow.ToString("yyyy-MM-dd");

                case "UTCTIME":
                    return DateTime.UtcNow.ToString("hh:mm:ss.sss");

                case "UTCNOW":
                    return DateTime.UtcNow.ToString("yyyy-MM-dd hh:mm:ss.sss");

                case "LOCALDATE":
                    return DateTime.Today.ToString("yyyy-MM-dd");

                case "LOCALTIME":
                    return DateTime.Now.ToString("hh:mm:ss.sss");

                case "LOCALNOW":
                    return DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.sss");

                case "DATEFORMAT":
                    return "yyyy-MM-dd";

                default:
                    var sessionToken = _sessionTokenAccessor.SessionToken();

                    if (_settingCacher.TryRead(sessionToken, setting, out var value)) return value.ToString();

                    return await _settings.ReadString(sessionToken, setting);
            }
        }
    }
}
