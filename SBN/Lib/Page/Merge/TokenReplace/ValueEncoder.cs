using System;
using System.Web;

namespace SBN.Lib.Page.Merge.TokenReplace
{
    public class ValueEncoder : IValueEncoder
    {
        public string Encode(string val, Definitions.EncodeType encodeType)
        {
            string ret;

            switch (encodeType)
            {
                case Definitions.EncodeType.HTML:
                    ret = HttpUtility.HtmlEncode(val);
                    break;

                case Definitions.EncodeType.URL:
                    ret = HttpUtility.UrlEncode(val);
                    break;

                case Definitions.EncodeType.None:
                    ret = val;
                    break;

                default:
                    throw new NotImplementedException();
            }

            return ret;
        }
    }
}