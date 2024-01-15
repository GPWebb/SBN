using System;
using System.Data;
using System.Data.SqlClient;
using AlpineRed.DB;

namespace SBN.Lib.DB.Lib
{
    public class DBUtil : IDBUtil
    {
        private readonly IConnectionStringReader _connectionStringReader;
        private readonly IConnector _connector;

        public DBUtil(IConnectionStringReader connectionStringReader, IConnector connector)
        {
            _connectionStringReader = connectionStringReader;
            _connector = connector;
        }

        public SqlConnection Connection => new SqlConnection(_connectionStringReader.ConnectionString());

        public IDbCommand GetAuthenticatedCommand(SqlConnection connection, Guid sessionToken, string commandText)
        {
            var command = _connector.CreateCommand(connection, commandText);
            command.Parameters.Add(Util.CreateParameter("SessionToken", SqlDbType.UniqueIdentifier, sessionToken));

            return command;
        }
    }
}
