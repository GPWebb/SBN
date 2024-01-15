using System;
using System.Data;
using System.Data.SqlClient;

namespace SBN.Lib.DB
{
    public interface IDBUtil
    {
        SqlConnection Connection { get; }
        IDbCommand GetAuthenticatedCommand(SqlConnection connection, Guid sessionToken, string commandText);
    }
}