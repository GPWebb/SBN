using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using AlpineRed.DB;
using SBN.Lib.Analytics;

namespace SBN.Lib.DB
{
    public class Analytics : IAnalytics
    {
        private readonly IConnector _connector;
        private readonly IDBUtil _dbUtil;

        public Analytics(IConnector connector,
            IDBUtil dbUtil)
        {
            _connector = connector;
            _dbUtil = dbUtil;
        }

        public async Task SetSessionLocation(Guid sessionToken, LocationData locationData)
        {
            var connection = _dbUtil.Connection;
            var command = (SqlCommand)_dbUtil.GetAuthenticatedCommand(connection, sessionToken, "analytics.SetSessionLocation");

            command.Parameters.Add(Util.CreateParameter("LatLong", SqlDbType.Udt, locationData.LatLong));
            command.Parameters[command.Parameters.Count - 1].UdtTypeName = "Geography";
            command.Parameters.Add(Util.CreateParameter("City", SqlDbType.NVarChar, locationData.City));
            command.Parameters.Add(Util.CreateParameter("Region", SqlDbType.NVarChar, locationData.Region));
            command.Parameters.Add(Util.CreateParameter("RegionCode", SqlDbType.NVarChar, locationData.RegionCode));
            command.Parameters.Add(Util.CreateParameter("RegionName", SqlDbType.NVarChar, locationData.RegionName));
            command.Parameters.Add(Util.CreateParameter("CountryCode", SqlDbType.NVarChar, locationData.CountryCode));
            command.Parameters.Add(Util.CreateParameter("CountryName", SqlDbType.NVarChar, locationData.CountryName));
            command.Parameters.Add(Util.CreateParameter("ContinentCode", SqlDbType.NVarChar, locationData.ContinentCode));
            command.Parameters.Add(Util.CreateParameter("ContinentName", SqlDbType.NVarChar, locationData.ContinentName));
            command.Parameters.Add(Util.CreateParameter("Timezone", SqlDbType.NVarChar, locationData.Timezone));

            await _connector.ExecuteCmd_NoReturn(connection, command);
        }

        public async Task RecordHit(Guid sessionToken, string context, string pathAndQuery, string referrer, DateTime hitDateTime)
        {
            var connection = _dbUtil.Connection;
            var command = (SqlCommand)_dbUtil.GetAuthenticatedCommand(connection, sessionToken, "analytics.Hit_Record");

            command.Parameters.Add(Util.CreateParameter("Context", SqlDbType.NVarChar, context));
            command.Parameters.Add(Util.CreateParameter("RequestPath", SqlDbType.NVarChar, pathAndQuery));
            command.Parameters.Add(Util.CreateParameter("Referrer", SqlDbType.NVarChar, referrer));
            command.Parameters.Add(Util.CreateParameter("RequestDT", SqlDbType.DateTime2, hitDateTime));

            await _connector.ExecuteCmd_NoReturn(connection, command);
        }

    }
}
