using Microsoft.AspNetCore.Http;
using SBN.Lib.Definitions;

namespace SBN.Lib.Request
{
    public class RequestReader : IRequestReader
    {
        public string SessionToken(HttpRequest request)
        {
            var sessionToken = request.Headers[Keys.SessionTokenHeaderKey].ToString();

            if (string.IsNullOrWhiteSpace(sessionToken))
            {
                sessionToken = request.Cookies[Keys.SessionTokenKey]?.ToString();
            }

            return sessionToken;
        }

        public string Referrer(HttpRequest request)
        {
            return request.Headers[Keys.Referrer].ToString();
        }

        public string PathAndQuery(HttpRequest request)
        {
            return request.Path + request.QueryString.ToUriComponent();
        }
    }
}
