using System.Threading.Tasks;

namespace SBN.Lib
{
    public interface ISettings
    {
        Task<string> GetSetting(string setting);
        void CacheSetting(string setting, string value);
    }
}