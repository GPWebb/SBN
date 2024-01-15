using System;
using System.Data;
using System.Threading.Tasks;
using AlpineRed.DB;
using Microsoft.AspNetCore.Http;
using SBN.Lib.Login;

namespace SBN.Lib.DB
{
    public class Logins : ILogins
    {
        private readonly IConnector _connector;
        private readonly IDBUtil _dbUtil;

        public Logins(IConnector connector, IDBUtil dbUtil)
        {
            _connector = connector;
            _dbUtil = dbUtil;
        }

        public async Task<Guid> GetStartSessionToken(HttpContext httpContext)
        {
            var connection = _dbUtil.Connection;
            var command = _connector.CreateCommand(connection, "auth.GetStartSessionToken");

            command.Parameters.Add(Util.CreateParameter("IPAddress", SqlDbType.NVarChar, httpContext.Connection.RemoteIpAddress.ToString()));
            command.Parameters.Add(Util.CreateParameter("UserAgent", SqlDbType.NVarChar, httpContext.Request.Headers["User-Agent"].ToString()));
            command.Parameters.Add(Util.CreateParameter("Referrer", SqlDbType.NVarChar, httpContext.Request.Headers["Referer"].ToString()));

            return (await _connector.GetDataRow(connection, command)).Field<Guid>(0);
        }

        public async Task<LoginResponse> Login(Guid sessionToken, string username, string password)
        {
            var connection = _dbUtil.Connection;
            var command = _dbUtil.GetAuthenticatedCommand(connection, sessionToken, "auth.DoLogin");

            command.Parameters.Add(Util.CreateParameter("Username", SqlDbType.NVarChar, username));
            command.Parameters.Add(Util.CreateParameter("Password", SqlDbType.NVarChar, password));

            var loginResult = (await _connector.ExecuteCmd_Respond(connection, command)).Data.Tables[0].Rows[0];

            return new LoginResponse(loginResult.Field<string>("LoginResult"), loginResult.Field<DateTime?>("SessionEnd"));
        }

        /// <summary>
        /// Log the user out and close their login session
        /// </summary>
        /// <remarks>Specifically _not_ awaited to ensure the logout has completed before proceeding</remarks>
        public void Logout(Guid sessionToken)
        {
            var connection = _dbUtil.Connection;
            var command = _dbUtil.GetAuthenticatedCommand(connection, sessionToken,"auth.DoLogout");

            _connector.ExecuteCmd_NoReturn(connection, command);
        }

        /// <remarks>Specifically _not_ awaited to ensure the logout has completed before proceeding</remarks>
        public void EnsureUnauthenticatedSessionLive(Guid sessionToken)
        {
            var connection = _dbUtil.Connection;
            var command = _dbUtil.GetAuthenticatedCommand(connection, sessionToken, "auth.EnsureUnauthenticatedSessionLive");

            _connector.ExecuteCmd_NoReturn(connection, command);
        }
    }
}
