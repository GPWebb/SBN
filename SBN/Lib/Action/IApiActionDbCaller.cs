using System;
using System.Threading.Tasks;
using System.Xml.Linq;
using SBN.Lib.Action.Outcome;

namespace SBN.Lib.Action
{
    public interface IApiActionDbCaller
    {
        Task<ApiActionOutcome> Call(Guid sessionToken, string pathAndQuery, string verb, string referrer, bool decorateForJson, XDocument requestBody);
    }
}