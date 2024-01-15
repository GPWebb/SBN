using Microsoft.AspNetCore.Http;
using SBN.Lib.Definitions;

namespace SBN.Lib.Sys
{
    public interface ILogger
    {
        void Log(string message, LogCategory logCategory, HttpRequest request);
    }
}