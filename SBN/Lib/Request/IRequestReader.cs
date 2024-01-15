using Microsoft.AspNetCore.Http;

namespace SBN.Lib.Request
{
    public interface IRequestReader
    {
        string SessionToken(HttpRequest request);
        string Referrer(HttpRequest request);
        string PathAndQuery(HttpRequest request);
    }
}