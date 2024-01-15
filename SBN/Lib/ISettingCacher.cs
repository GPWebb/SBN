using System;

namespace SBN.Lib
{
    public interface ISettingCacher
    {
        void Write(Guid sessionToken, string setting, string value);
        bool TryRead(Guid sessionToken, string setting, out string value);
        string Read(Guid sessionToken, string setting);
    }
}