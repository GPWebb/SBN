using SBN.Lib.Definitions;
using SBN.Lib.Page.Request;
using System;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SBN.Lib.Page.Merge
{
    public interface INonDataMerger
    {
        Task<string> MergeNonData(string input,
            string pageTitle,
            Guid sessionToken,
            ParamEncode urlEncode = ParamEncode.PassThrough,
            EncodeType encodeType = EncodeType.None,
            ValidatedRequest requestParams = null);

        Task<XElement> MergeNonDataXml(XElement input,
            string pageTitle,
            ParamEncode urlEncode,
            EncodeType encodeType,
            Guid sessionToken,
            ValidatedRequest requestParams = null);
    }
}