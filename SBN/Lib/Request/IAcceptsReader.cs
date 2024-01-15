using Microsoft.AspNetCore.Http;
using SBN.Lib.Definitions;

namespace SBN.Lib.Request
{
    public interface IAcceptsReader
    {
        ResponseDataType Read(HttpRequest request);
    }
}