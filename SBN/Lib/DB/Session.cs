using AlpineRed.DB;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SBN.Lib.DB
{
    public class Session : ISession
    {
        private readonly IConnector _connector;
        private readonly IDBUtil _dbUtil;

        public Session(IConnector connector,
            IDBUtil dbUtil)
        {
            _connector = connector;
            _dbUtil = dbUtil;
        }

        public async Task<SessionDetails> SessionDetails(Guid sessionToken)
        {
            var connection = _dbUtil.Connection;
            var command = (SqlCommand)_dbUtil.GetAuthenticatedCommand(connection, sessionToken, "app.SessionDetails");

            var session = await _connector.GetDataSet(connection, command);

            return new SessionDetails(session);
        }
    }
}
