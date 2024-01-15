using System;
using System.Threading.Tasks;
using System.Xml.Linq;
using SBN.Lib.Action.Data;
using SBN.Lib.Page.Request;

namespace SBN.Lib.DB
{
    public interface IActions
    {
        Task<ActionReturnData> Call(Guid sessionToken,
            string pathAndQuery,
            string verb,
            string referrer,
            XDocument requestBody);

        Task<ActionData> CallByID(Guid sessionToken,
            int actionID,
            string requestPath,
            ValidatedRequest validatedRequest);

        Task<ActionData> CallByID(Guid sessionToken,
            int actionID,
            string requestPath,
            string requestParameters);
    }
}