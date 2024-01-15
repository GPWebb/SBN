using System.Data.Common;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SBN.Lib.Sys;

namespace SBN.Lib.DB.Lib
{
    public class ConnectionStringReader : IConnectionStringReader
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOptions<EnvironmentConfig> _environmentConfig;

        public ConnectionStringReader(IHttpContextAccessor httpContextAccessor,
            IOptions<EnvironmentConfig> environmentConfig)
        {
            _httpContextAccessor = httpContextAccessor;
            _environmentConfig = environmentConfig;
        }

        public string ConnectionString(string connectionString, string host)
        {
            if (!string.IsNullOrWhiteSpace(connectionString)) return connectionString;

            var environment = _environmentConfig.Value.Environments.Single(x => x.Hosts.Any(h => h == host));

            DbConnectionStringBuilder csb;
            if (string.IsNullOrWhiteSpace(environment.UserID))
            {
                csb = new DbConnectionStringBuilder
                {
                    { "Data Source", environment.DataSource },
                    { "Initial Catalog", environment.InitialCatalog },
                    { "Integrated Security", "SSPI" }
                };
            }
            else
            {
                csb = new DbConnectionStringBuilder
                {
                    { "Data Source", environment.DataSource },
                    { "Initial Catalog", environment.InitialCatalog },
                    { "User ID", environment.UserID },
                    { "Password", environment.Password }
                };
            }

            return csb.ConnectionString;
        }


        public string ConnectionString()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            var connectionString = httpContext.Session?.GetString("ConnectionString");

            var host = httpContext.Request.Host.ToString();

            connectionString = ConnectionString(connectionString, host);

            //HACK TrySetString shouldn't be needed, just randomly broke - this is a plaster to unblock me
            httpContext.Session.TrySetString("ConnectionString",  connectionString);
            return connectionString;
        }
    }
}
