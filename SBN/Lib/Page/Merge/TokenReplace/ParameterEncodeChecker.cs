using System;

namespace SBN.Lib.Page.Merge.TokenReplace
{
    public class ParameterEncodeChecker : IParameterEncodeChecker
    {
        public bool CheckEncodeParameter(ref string parameter, Definitions.ParamEncode encode)
        {
            switch (encode)
            {
                case Definitions.ParamEncode.Encode:
                    return true;

                case Definitions.ParamEncode.PassThrough:
                    return false;

                case Definitions.ParamEncode.PassThroughPrefixed:
                    if (!parameter.StartsWith(".")) return true;

                    parameter = parameter.Remove(0, 1);
                    return false;

                default:
                    throw new InvalidOperationException($"Unrecognised encode type '{ encode }'");
            }
        }
    }
}
