using System;
using System.Data;
using System.Data.SqlClient;
using AlpineRed.DB;
using SBN.Lib.Definitions;

namespace SBN.Lib.DB
{
    public class DBLogger : IDBLogger
    {
        private readonly IConnector _connector;
        private readonly IDBUtil _dbUtil;

        public DBLogger(IConnector connector,
            IDBUtil dbUtil)
        {
            _connector = connector;
            _dbUtil = dbUtil;
        }

        public void Log(Guid sessionToken,
            LogCategory logCategory,
            string message,
            string logURL,
            string referrerURL)
        {
            var connection = _dbUtil.Connection;
            var command = (SqlCommand)_dbUtil.GetAuthenticatedCommand(connection, sessionToken, "app.RequestLog_Add");

            command.Parameters.Add(Util.CreateParameter("RequestLogCategory", SqlDbType.NVarChar, logCategory.ToString())); ;
            command.Parameters.Add(Util.CreateParameter("Message", SqlDbType.NVarChar, message));
            command.Parameters.Add(Util.CreateParameter("LogURL", SqlDbType.NVarChar, logURL));
            command.Parameters.Add(Util.CreateParameter("ReferrerURL", SqlDbType.NVarChar, referrerURL));

            _connector.ExecuteCmd_NoReturn(connection, command);
        }
    }
}
