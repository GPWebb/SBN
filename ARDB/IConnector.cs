using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace AlpineRed.DB
{
    public interface IConnector
    {
        Task<SqlConnection> ConnectionOpen(SqlConnection sqlConnection);

        void ConnectionClose(SqlConnection sqlConnection);

        Task<DataTable> GetDataTable(SqlConnection sqlConnection, string query);

        Task<DataTable> GetDataTable(SqlConnection sqlConnection, IDbCommand command);

        Task<DataSet> GetDataSet(SqlConnection sqlConnection, string query);

        Task<DataSet> GetDataSet(SqlConnection sqlConnection, IDbCommand command);

        Task<DataRow> GetDataRow(SqlConnection sqlConnection, IDbCommand command);

        Task<DataRow> GetDataRow(SqlConnection sqlConnection, string query);

        Task<object> ExecuteScalar(SqlConnection sqlConnection, string query);

        Task<object> ExecuteScalar(SqlConnection sqlConnection, IDbCommand command);

        Task ExecuteSQL_NoReturn(SqlConnection sqlConnection, string query);

        Task ExecuteCmd_NoReturn(SqlConnection sqlConnection, IDbCommand command);

        Task<int> ExecuteSQL_GetReturnVal(SqlConnection sqlConnection, string query);

        Task<int> ExecuteCmd_GetReturnVal(SqlConnection sqlConnection, IDbCommand command);

        Task<DbResponse<DataSet>> ExecuteCmd_Respond(SqlConnection sqlConnection, IDbCommand command);

        IDbCommand CreateCommand(SqlConnection sqlConnection, string command);
    }
}