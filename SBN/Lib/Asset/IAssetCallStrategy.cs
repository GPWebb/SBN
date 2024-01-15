using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace SBN.Lib.Asset
{
    public interface IAssetCallStrategy
    {
        bool Selector(string method);

        Task<IActionResult> Call(HttpRequest request);
    }
}
