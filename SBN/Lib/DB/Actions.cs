using System;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using AlpineRed.DB;
using SBN.Lib.Action.Data;
using SBN.Lib.Page.Outcome;
using SBN.Lib.Page.Request;

namespace SBN.Lib.DB
{
    public class Actions : IActions
    {
        private readonly IConnector _connector;
        private readonly IDBUtil _dbUtil;

        public Actions(IConnector connector,
            IDBUtil dbUtil)
        {
            _connector = connector;
            _dbUtil = dbUtil;
        }

        public async Task<ActionReturnData> Call(Guid sessionToken,
            string pathAndQuery,
            string verb,
            string referrer,
            XDocument requestBody)
        {
            var connection = _dbUtil.Connection;
            var command = (SqlCommand)_dbUtil.GetAuthenticatedCommand(connection, sessionToken, "app.Action_ExecFromRequest");

            command.Parameters.Add(Util.CreateParameter("PathAndQuery", SqlDbType.NVarChar, pathAndQuery));
            command.Parameters.Add(Util.CreateParameter("Verb", SqlDbType.NVarChar, verb));
            command.Parameters.Add(Util.CreateParameter("Referrer", SqlDbType.NVarChar, referrer));
            command.Parameters.Add(Util.CreateParameter("RequestBody", SqlDbType.NVarChar, requestBody?.Root.ToString()));

            command.Parameters.Add(Util.CreateParameter("Definition", SqlDbType.Xml, ParameterDirection.Output, null));
            command.Parameters.Add(Util.CreateParameter("Transform", SqlDbType.Xml, ParameterDirection.Output, null));

            var result = await _connector
                .GetDataSet(connection, command);

            return new ActionReturnData
            {
                DataSet = result,
                Definition = command.Parameters["Definition"].Value.ToString(),
                Transform = command.Parameters["Transform"].Value.ToString()
            };
        }

        public async Task<ActionData> CallByID(Guid sessionToken, int actionID, string requestPath, ValidatedRequest validatedRequest)
        {
            return await CallByID(sessionToken, actionID, requestPath, validatedRequest.AllValues());
        }

        public async Task<ActionData> CallByID(Guid sessionToken, int actionID, string requestPath, string requestParameters)
        {
            var connection = _dbUtil.Connection;
            var command = (SqlCommand)_dbUtil.GetAuthenticatedCommand(connection, sessionToken, "app.ActionExec");

            command.Parameters.Add(Util.CreateParameter("ActionID", SqlDbType.Int, actionID));
            command.Parameters.Add(Util.CreateParameter("QueryString", SqlDbType.NVarChar, requestParameters));
            command.Parameters.Add(Util.CreateParameter("PathAndQuery", SqlDbType.NVarChar, requestPath));

            var result = await _connector
                .GetDataSet(connection, command);

            return new ActionData
            {
                ActionID = actionID,
                Data = result,
                StatusCode = HttpStatusCode.OK
            };
        }
    }
}
