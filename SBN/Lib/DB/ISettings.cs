using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SBN.Lib.DB
{
    public interface ISettings
    {
        Task<T> Read<T>(Guid sessionToken, string settingName);
        Task<string> ReadString(Guid sessionToken, string settingName);
        Task<DataTable> GetInitialSettings(Guid sessionToken);
    }
}