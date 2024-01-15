using SBN.Lib.Page.Request;
using System;
using System.Threading.Tasks;

namespace SBN.Lib.Action.Data
{
    public interface IActionDataLoader
    {
        Task<ActionData> Load(ActionData actionData,
                    Guid sessionToken,
                    string requestPath);
    }
}