using System;
using System.Data;

namespace SBN.Lib.Page
{
    public interface IProfileDataCacher
    {
        void CacheProfileData(Guid sessionToken, DataRow profileData);
    }
}
