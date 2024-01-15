using SBN.Models;
using System;
using System.Threading.Tasks;

namespace SBN.Lib.Action.Data
{
    public interface IApiDataLoader
    {
        Task<Response> Get(ActionData actionData,
                    Guid sessionToken,
                    string requestPath);
    }
}