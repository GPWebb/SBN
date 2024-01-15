using System;
using System.Net;

namespace SBN.Lib.Sys
{
    public class RestCallException : Exception
    {
        public string Uri { get; private set; }
        public string Method { get; private set; }
        public HttpStatusCode ResponseStatusCode { get; private set; }

        public RestCallException(string uri, string method, HttpStatusCode statusCode, string message)
            : base(message)
        {
            Uri = uri;
            Method = method;
            ResponseStatusCode = statusCode;
        }
    }
}
