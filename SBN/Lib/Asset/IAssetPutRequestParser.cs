using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace SBN.Lib.Asset
{
    public interface IAssetPutRequestParser
    {
        AssetPutRequest Parse(HttpRequest request);
    }
}