using AlpineRed.DB;
using Microsoft.Extensions.Options;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SBN.Lib.DB
{
    public class Pages : IPages
    {
        private readonly IOptions<EnvironmentConfig> _environmentConfig;
        private readonly IConnector _connector;
        private readonly IConnectionStringReader _connectionStringReader;
        private readonly IDBUtil _dbUtil;

        public Pages(IOptions<EnvironmentConfig> environmentConfig,
            IConnector connector,
            IConnectionStringReader connectionStringReader,
            IDBUtil dbUtil)
        {
            _environmentConfig = environmentConfig;
            _connector = connector;
            _connectionStringReader = connectionStringReader;
            _dbUtil = dbUtil;
        }

        public async Task<DataSet> Get(Guid sessionToken, string pathAndQuery, string referrer)
        {
            var connection = await _connector.ConnectionOpen(new SqlConnection(_connectionStringReader.ConnectionString()));
            var command = _dbUtil.GetAuthenticatedCommand(connection, sessionToken, "site.PageDetails");

            command.Parameters.Add(Util.CreateParameter("PathAndQuery", SqlDbType.NVarChar, pathAndQuery));
            command.Parameters.Add(Util.CreateParameter("Referrer", SqlDbType.NVarChar, referrer));
            command.Parameters.Add(Util.CreateParameter("ReturnData", SqlDbType.Bit, _environmentConfig.Value.PageReturnData));

            var result = await _connector.GetDataSet(connection, command);

            return result;
        }
    }
}
