using System;
using SBN.Lib.Definitions;

namespace SBN.Lib.DB
{
    public interface IDBLogger
    {
        void Log(Guid sessionToken,
            LogCategory logCategory,
            string message,
            string logURL,
            string referrerURL);
    }
}