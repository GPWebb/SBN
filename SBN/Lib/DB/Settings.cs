using System;
using System.Data;
using System.Threading.Tasks;
using AlpineRed.DB;
using static AlpineRed.DB.Util;

namespace SBN.Lib.DB
{
    public class Settings : ISettings
    {
        private readonly IDBUtil _dbUtil;
        private readonly IConnector _connector;

        public Settings(IDBUtil dbUtil, IConnector connector)
        {
            _dbUtil = dbUtil;
            _connector = connector;
        }

        public async Task<T> Read<T>(Guid sessionToken, string settingName)
        {
            var connection = _dbUtil.Connection;
            var command = _dbUtil.GetAuthenticatedCommand(connection, sessionToken, "app.GetSetting");
            command.Parameters.Add(CreateParameter("SettingName", SqlDbType.NVarChar, settingName));

            var settings = await _connector.GetDataRow(connection, command);

            if (typeof(T) == typeof(int)) return settings.Field<T>("SettingValue_int");

            if (typeof(T) == typeof(string)) return settings.Field<T>("SettingValue_Text");

            if (typeof(T) == typeof(DateTime)) return settings.Field<T>("SettingValue_DateTime");

            throw new ArgumentException($"Unsupported setting type '{typeof(T).Name}'");
        }

        public async Task<string> ReadString(Guid sessionToken, string settingName)
        {
            var connection = _dbUtil.Connection;
            var command = _dbUtil.GetAuthenticatedCommand(connection, sessionToken, "app.Setting_Read");
            command.Parameters.Add(CreateParameter("SettingName", SqlDbType.NVarChar, settingName));

            var settings = await _connector.GetDataRow(connection, command);

            switch (settings.Field<string>("SettingType").ToUpperInvariant())
            {
                case "TEXT":
                    return settings.Field<string>("SettingValue_Text");
                case "INT":
                    return settings.Field<int>("SettingValue_Int").ToString();
                case "DATETIME":
                    return settings.Field<DateTime>("SettingValue_DateTime").ToString("yyyy-MM-dd hh:mm:ss.sss");
                default:
                    throw new ArgumentException($"Unsupported setting type '{settings.Field<string>("SettingType")}'");
            }
        }

        public async Task<DataTable> GetInitialSettings(Guid sessionToken)
        {
            var connection = _dbUtil.Connection;

            var command = _dbUtil.GetAuthenticatedCommand(connection, sessionToken, "app.GetInitialSettings");

            return await _connector.GetDataTable(connection, command);
        }
    }
}
