using SBN.Lib.Definitions;
using SBN.Lib.Page.Merge.TokenReplace;
using SBN.Lib.Page.Request;
using System;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SBN.Lib.Page.Merge
{
    public class NonDataMerger : INonDataMerger
    {
        private readonly ITokenReplacer _tokenReplacer;

        public NonDataMerger(ITokenReplacer tokenReplacer)
        {
            _tokenReplacer = tokenReplacer;
        }

        public async Task<string> MergeNonData(string input,
            string pageTitle,
            Guid sessionToken,
            ParamEncode urlEncode = ParamEncode.PassThrough,
            EncodeType encodeType = EncodeType.None,
            ValidatedRequest requestParams = null)
        {
            try
            {
                input = await _tokenReplacer.Replace(input, urlEncode, encodeType, requestParams, pageTitle, sessionToken);

                return input;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error performing non-data merge: {ex.Message}", ex);
            }
        }

        public async Task<XElement> MergeNonDataXml(XElement input,
            string pageTitle,
            ParamEncode urlEncode,
            EncodeType encodeType,
            Guid sessionToken,
            ValidatedRequest requestParams = null)
        {
            return XElement.Parse(await MergeNonData(input.ToString(), pageTitle, sessionToken, urlEncode, encodeType, requestParams));
        }
    }
}
