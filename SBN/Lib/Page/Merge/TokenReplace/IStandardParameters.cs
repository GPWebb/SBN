using System;
using System.Data;
using System.Threading.Tasks;

namespace SBN.Lib.Page.Merge.TokenReplace
{
    public interface IStandardParameters
    {
        Task<string> StdParams(string input,
                    Definitions.ParamEncode encode,
                    Definitions.EncodeType encodeType,
                    string pageTitle,
                    Guid sessionToken,
                    bool passThrough = false);
    }
}