using System;
using Microsoft.AspNetCore.Http;
using SBN.Lib.DB;
using SBN.Lib.Definitions;
using SBN.Lib.Request;

namespace SBN.Lib.Sys
{
    public class Logger : ILogger
    {
        private readonly IDBLogger _dbLogger;
        private readonly ISessionTokenAccessor _sessionTokenAccessor;
        private readonly IRequestReader _requestReader;

        public Logger(IDBLogger dbLogger, 
            ISessionTokenAccessor sessionTokenAccessor,
            IRequestReader requestReader)
        {
            _dbLogger = dbLogger;
            _sessionTokenAccessor = sessionTokenAccessor;
            _requestReader = requestReader;
        }

        public void Log(string message, LogCategory logCategory, HttpRequest request)
        {
            try
            {
                _dbLogger.Log(_sessionTokenAccessor.SessionToken(),
                    logCategory,
                    message,
                    _requestReader.PathAndQuery(request),
                    _requestReader.Referrer(request));
            }
            catch(Exception ex)
            {
                //swallow, last thing we want is an exception log failure to cause a crash
                _ = ex;
            }
        }
    }
}
