using System.Threading.Tasks;
using System;
using SBN.Lib.DB;
using Microsoft.Extensions.Options;

namespace SBN.Lib.Page.Call
{
    public class EstablishSessionToken : IEstablishSessionToken
    {
        private readonly IOptions<EnvironmentConfig> _environmentConfig;
        private readonly ISessionTokenAccessor _sessionTokenAccessor;
        private readonly ILogins _logins;

        public EstablishSessionToken(IOptions<EnvironmentConfig> environmentConfig,
            ISessionTokenAccessor sessionTokenAccessor,
            ILogins logins)
        {
            _environmentConfig = environmentConfig;
            _sessionTokenAccessor = sessionTokenAccessor;
            _logins = logins;
        }

        public async Task<Guid> Establish()
        {
            Guid sessionToken = _sessionTokenAccessor.SessionToken();
            if (_environmentConfig.Value.IndividualUnauthenticatedSessions)
            {
                if (sessionToken == Guid.Empty) sessionToken = await _sessionTokenAccessor.SetSessionToken();
            }
            else
            {
                _logins.EnsureUnauthenticatedSessionLive(sessionToken);
            }

            return sessionToken;
        }
    }
}
