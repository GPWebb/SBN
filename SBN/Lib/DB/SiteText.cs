using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using AlpineRed.DB;

namespace SBN.Lib.DB
{
    public class SiteText : ISiteText
    {
        private readonly IDBUtil _dbUtil;
        private readonly IConnector _connector;

        public SiteText(IDBUtil dbUtil,
            IConnector connector)
        {
            _dbUtil = dbUtil;
            _connector = connector;
        }

        public async Task<IDictionary<string, string>> Read(Guid sessionToken, IEnumerable<string> siteTextEntries)
        {
            var connection = _dbUtil.Connection;
            var command = (SqlCommand)_dbUtil.GetAuthenticatedCommand(connection, sessionToken, "site.SiteText_ReadMultiple");

            command.Parameters.Add(Util.CreateParameter("SiteTextEntries", SqlDbType.NVarChar, string.Join("|", siteTextEntries)));

            var entries = await _connector
                .GetDataTable(connection, command);

            var result = new Dictionary<string, string>();

            foreach(DataRow entry in entries.Rows)
            {
                result.Add(entry.Field<string>("Alias"), entry.Field<string>("Pattern"));
            }

            return result;
        }
    }
}
