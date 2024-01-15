using SBN.Lib.Page.Request;
using System;
using System.Threading.Tasks;

namespace SBN.Lib.Page.Merge.TokenReplace
{
    public interface ITokenReplacer
    {
        Task<string> Replace(string input, Definitions.ParamEncode encode, Definitions.EncodeType encodeType, string pageTitle, Guid sessionToken);
        Task<string> Replace(string input, Definitions.ParamEncode encode, Definitions.EncodeType encodeType, ValidatedRequest requestParams, string pageTitle, Guid sessionToken);
    }
}