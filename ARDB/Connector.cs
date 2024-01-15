using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AlpineRed.DB
{
    public class Connector : IConnector
    {
        private const int _deadlockCode = 1205;
        private const int _maxRetries = 10;

        private enum DataAction
        {
            DataSet,
            DataReader,
            ExecuteScalar,
            ExecuteNoReturn,
            ReturnCode
        }

        #region "Connection management"
        public async Task<SqlConnection> ConnectionOpen(SqlConnection sqlConnection)
        {
            if (sqlConnection.State == ConnectionState.Open) return sqlConnection;

            var i = _maxRetries;

            while (sqlConnection.State == ConnectionState.Closed)
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    return sqlConnection;
                }
                catch (Exception ex)
                {
                    if (i-- >= 0 &&
                            ex.Message == "Invalid operation. The connection is closed." |
                            ex.Message.StartsWith("Not allowed to change the 'ConnectionString' property. The connection's current state is ")
                        )
                    {
                        Thread.Sleep(new Random().Next(50, 250));
                    }
                    else throw;
                }
            }

            throw new Exception("This shouldn't be reachable");
        }

        public void ConnectionClose(SqlConnection sqlConnection)
        {
            try
            {
                sqlConnection.Close();
            }
            catch //(Exception ex)
            {
                //Do nothing
            }
        }
        #endregion


        #region "Handle database interaction and error handling"
        private async Task<object> DBInteract(SqlConnection sqlConnection, string query, DataAction v_enuDataAction)
        {
            sqlConnection = await ConnectionOpen(sqlConnection);

            int retries = _maxRetries;
            var completed = false;
            object ret = null;

            while (!completed)
            {
                try
                {
                    switch (v_enuDataAction)
                    {
                        case DataAction.DataReader:
                            ret = new SqlCommand(query, sqlConnection).ExecuteReader(CommandBehavior.CloseConnection);
                            break;

                        case DataAction.DataSet:
                            SqlDataAdapter da = new SqlDataAdapter(query, sqlConnection);
                            DataSet ds = new DataSet();
                            da.Fill(ds);
                            ret = ds;
                            break;

                        case DataAction.ExecuteNoReturn:
                            IDbCommand cmd = new SqlCommand(query, sqlConnection);
                            cmd.ExecuteNonQuery();
                            break;

                        case DataAction.ExecuteScalar:
                            ret = new SqlCommand(query, sqlConnection).ExecuteScalar();
                            break;

                        case DataAction.ReturnCode:
                            IDbCommand cmdr = new SqlCommand(query, sqlConnection);
                            SqlParameter rtnVal = new SqlParameter { Direction = ParameterDirection.ReturnValue };
                            cmdr.Parameters.Add(rtnVal);
                            cmdr.ExecuteNonQuery();
                            ret = (int)rtnVal.Value;
                            break;

                        default:
                            throw new Exception($"Unrecognised data action '{v_enuDataAction}'");
                    }
                }
                catch (SqlException dbex)
                {
                    if (!DBExceptionHandler(dbex, ref retries))
                        throw;
                }
            }

            ConnectionClose(sqlConnection);
            return ret;
        }

        private async Task<DbResponse<object>> DBInteract(SqlConnection sqlConnection, IDbCommand cmd, DataAction dataAction)
        {
            int returnCode = 0;

            sqlConnection = await ConnectionOpen(sqlConnection);

            var retries = _maxRetries;
            cmd.Connection = sqlConnection;
            cmd.CommandType = CommandType.StoredProcedure;

            SqlParameter rtnCode = new SqlParameter { Direction = ParameterDirection.ReturnValue };
            cmd.Parameters.Add(rtnCode);

            object ret = null;
            var completed = false;

            while (!completed)
            {
                try
                {
                    switch (dataAction)
                    {
                        case DataAction.DataReader:
                            ret = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                            returnCode = (int)rtnCode.Value;
                            completed = true;
                            break;

                        case DataAction.DataSet:
                            SqlDataAdapter da = new SqlDataAdapter();
                            da.SelectCommand = (SqlCommand)cmd;
                            DataSet ds = new DataSet();
                            da.Fill(ds);
                            returnCode = (int)rtnCode.Value;
                            ret = ds;
                            completed = true;
                            break;

                        case DataAction.ExecuteNoReturn:
                            cmd.ExecuteNonQuery();
                            returnCode = (int)rtnCode.Value;
                            completed = true;
                            break;

                        case DataAction.ExecuteScalar:
                            ret = cmd.ExecuteScalar();
                            returnCode = (int)rtnCode.Value;
                            completed = true;
                            break;

                        case DataAction.ReturnCode:
                            cmd.ExecuteNonQuery();
                            ret = (int)rtnCode.Value;
                            completed = true;
                            break;

                        default:
                            throw new Exception($"Unrecognised data action '{dataAction.ToString()}'");
                    }
                }
                catch (SqlException dbex)
                {
                    if (!DBExceptionHandler(dbex, ref retries))
                    {
                        if (dbex.Message.StartsWith("Procedure or function '") & dbex.Message.Contains("' expects parameter '") & dbex.Message.EndsWith("', which was not supplied."))
                            DBParameterExceptionHandler(dbex, ((SqlCommand)cmd).Parameters);
                        else
                            throw;
                    }
                }
            }

            ConnectionClose(sqlConnection);

            return new DbResponse<object> { Data = ret, ReturnCode = returnCode };
        }

        private void DBParameterExceptionHandler(SqlException DBEx, SqlParameterCollection parameters)
        {
            var err = new StringBuilder(DBEx.Message);
            err.AppendLine();
            err.AppendLine("Supplied parameters:");

            foreach (SqlParameter p in parameters)
            {
                try
                {
                    string val;
                    if (p.Value == null || p.Value == DBNull.Value)
                        val = "NULL";
                    else
                        val = p.Value.ToString();

                    err.AppendLine($"{p.ParameterName}:\"{val}\"");
                }
                catch (Exception ex)
                {
                    err.AppendLine(ex.Message);
                }
            }
            throw new Exception(err.ToString(), DBEx);

        }

        private bool DBExceptionHandler(SqlException DBex, ref int retries)
        {
            //Should this also handle timeouts?
            if (!(DBex.Number == _deadlockCode |
                  DBex.Message ==
                  "There is already an open DataReader associated with this Command which must be closed first."))
                return false;

            if (retries >= _maxRetries) return false;

            Random rnd = new Random();
            retries++;
            Thread.Sleep(rnd.Next(50, 500));
            return true;
        }
        #endregion

        #region "Front end routines for interacting with the database"
        public async Task<DataTable> GetDataTable(SqlConnection sqlConnection, string query)
        {
            return (await GetDataSet(sqlConnection, query)).Tables[0];
        }
        public async Task<DataTable> GetDataTable(SqlConnection sqlConnection, IDbCommand command)
        {
            return (await GetDataSet(sqlConnection, command)).Tables[0];
        }

        public async Task<DataSet> GetDataSet(SqlConnection sqlConnection, string query)
        {
            return (DataSet)(await DBInteract(sqlConnection, query, DataAction.DataSet));
        }
        public async Task<DataSet> GetDataSet(SqlConnection sqlConnection, IDbCommand command)
        {
            return (DataSet)(await DBInteract(sqlConnection, command, DataAction.DataSet)).Data;
        }

        public async Task<DataRow> GetDataRow(SqlConnection sqlConnection, IDbCommand command)
        {
            return (await GetDataSet(sqlConnection, command)).Tables[0].Rows[0];
        }
        public async Task<DataRow> GetDataRow(SqlConnection sqlConnection, string query)
        {
            return (await GetDataSet(sqlConnection, query)).Tables[0].Rows[0];
        }

        public async Task<object> ExecuteScalar(SqlConnection sqlConnection, string query)
        {
            return (await DBInteract(sqlConnection, query, DataAction.ExecuteScalar));
        }
        public async Task<object> ExecuteScalar(SqlConnection sqlConnection, IDbCommand command)
        {
            return (await DBInteract(sqlConnection, command, DataAction.ExecuteScalar));
        }

        public async Task ExecuteSQL_NoReturn(SqlConnection sqlConnection, string query)
        {
            await DBInteract(sqlConnection, query, DataAction.ExecuteNoReturn);
        }

        public async Task ExecuteCmd_NoReturn(SqlConnection sqlConnection, IDbCommand command)
        {
            await DBInteract(sqlConnection, command, DataAction.ExecuteNoReturn);
        }

        public async Task<int> ExecuteSQL_GetReturnVal(SqlConnection sqlConnection, string query)
        {
            return (int)(await DBInteract(sqlConnection, query, DataAction.ReturnCode));
        }

        public async Task<int> ExecuteCmd_GetReturnVal(SqlConnection sqlConnection, IDbCommand command)
        {
            return (int)(await DBInteract(sqlConnection, command, DataAction.ReturnCode)).ReturnCode;
        }

        public async Task<DbResponse<DataSet>> ExecuteCmd_Respond(SqlConnection sqlConnection, IDbCommand command)
        {
            var response = await DBInteract(sqlConnection, command, DataAction.DataSet);

            if (response.Data != null) return new DbResponse<DataSet> { Data = (DataSet)response.Data, ReturnCode = response.ReturnCode };

            return null;
        }
        #endregion

        public IDbCommand CreateCommand(SqlConnection sqlConnection, string command)
        {
            return new SqlCommand
            {
                Connection = sqlConnection,
                CommandType = CommandType.StoredProcedure,
                CommandText = command
            };
        }
    }
}