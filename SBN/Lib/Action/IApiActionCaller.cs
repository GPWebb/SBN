using Microsoft.AspNetCore.Http;
using SBN.Lib.Action.Outcome;
using System.Threading.Tasks;

namespace SBN.Lib.Action
{
    public interface IApiActionCaller
    {
        Task<ApiActionOutcome> Call(HttpRequest request);
    }
}