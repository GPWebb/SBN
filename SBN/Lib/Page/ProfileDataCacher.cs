using System;
using System.Data;

namespace SBN.Lib.Page
{
    public class ProfileDataCacher : IProfileDataCacher
    {
        private readonly ISettingCacher _settingCacher;

        public ProfileDataCacher(ISettingCacher settingCacher)
        {
            _settingCacher = settingCacher;
        }

        public void CacheProfileData(Guid sessionToken, DataRow profileData)
        {
            for (var i = 0; i < profileData.Table.Columns.Count; i++)
            {
                _settingCacher.Write(sessionToken, profileData.Table.Columns[i].ColumnName, profileData[i].ToString());
            }
        }
    }
}
