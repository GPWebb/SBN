using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SBN.Lib.Analytics;
using SBN.Lib.DB;
using SBN.Lib.Definitions;
using SBN.Lib.Request;

namespace SBN.Lib
{
    public class SessionTokenAccessor : ISessionTokenAccessor
    {
        private readonly IOptions<EnvironmentConfig> _environmentConfig;
        private readonly IRequestReader _requestReader;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogins _logins;
        private readonly IGeolocator _geolocator;

        public SessionTokenAccessor(IOptions<EnvironmentConfig> environmentConfig,
            IRequestReader requestReader,
            IHttpContextAccessor httpContextAccessor,
            ILogins logins,
            IGeolocator geolocator)
        {
            _environmentConfig = environmentConfig;
            _requestReader = requestReader;
            _httpContextAccessor = httpContextAccessor;
            _logins = logins;
            _geolocator = geolocator;
        }

        public Guid SessionToken()
        {
            var sessionToken = _requestReader.SessionToken(_httpContextAccessor.HttpContext.Request);
            if (string.IsNullOrWhiteSpace(sessionToken))
                sessionToken = _httpContextAccessor.HttpContext.Session.GetString(Keys.SessionTokenKey);

            return Guid.TryParse(sessionToken, out Guid tokenGuid)
                ? tokenGuid
                : Guid.Empty;
        }

        public async Task<Guid> SetSessionToken()
        {
            var context = _httpContextAccessor.HttpContext;

            var sessionToken = await _logins.GetStartSessionToken(context);
            SetSessionTokenInternal(sessionToken);

            _geolocator.Set(sessionToken, context);

            return sessionToken;
        }

        public void SetBlankSessionToken()
        {
            if (_environmentConfig.Value.IndividualUnauthenticatedSessions) throw new Exception("Cannot set blank token, individual sessions are required");

            var empty = Guid.Empty;
            SetSessionTokenInternal(empty);
        }

        private void SetSessionTokenInternal(Guid sessionToken)
        {
            var cookieOptions = new CookieOptions
            {
                Expires = new DateTimeOffset(DateTime.Now.AddMinutes(_environmentConfig.Value.SessionCookieExpiryMinutes)),
                HttpOnly = true,
                SameSite = SameSiteMode.Lax
            };

            _httpContextAccessor.HttpContext.Session.SetString(Keys.SessionTokenKey, sessionToken.ToString());
            _httpContextAccessor.HttpContext.Response.Cookies.Append(Keys.SessionTokenKey, sessionToken.ToString(), cookieOptions);
        }

        public void ResetSessionToken()
        {
            if (!_environmentConfig.Value.IndividualUnauthenticatedSessions)
            {
                SetBlankSessionToken();
            }
            else
            {
                SetSessionToken();
            }
        }
    }
}
