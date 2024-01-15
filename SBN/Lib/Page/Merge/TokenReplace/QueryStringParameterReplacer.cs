using System;
using Microsoft.AspNetCore.Http;
using SBN.Lib.Page.Request;

namespace SBN.Lib.Page.Merge.TokenReplace
{
    public class QueryStringParameterReplacer : IQueryStringParameterReplacer
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IValueEncoder _valueEncoder;
        private readonly IParameterEncodeChecker _parameterEncodeChecker;

        public QueryStringParameterReplacer(IHttpContextAccessor contextAccessor,
            IValueEncoder valueEncoder,
            IParameterEncodeChecker parameterEncodeChecker)
        {
            _contextAccessor = contextAccessor;
            _valueEncoder = valueEncoder;
            _parameterEncodeChecker = parameterEncodeChecker;
        }

        public string Replace(ValidatedRequest requestParams, 
            Definitions.ParamEncode encode, 
            Definitions.EncodeType encodeType, 
            string output,
            Definitions.ParamMissingBehaviour paramMissingBehaviour)
        {
            string value;
            var position = output.IndexOf("[?");
            var parameter = output.Substring(position + 2, output.IndexOf("]", position) - position - 2);

            bool prefix = false;
            if(parameter.StartsWith("."))
            {
                prefix = true;
                parameter = parameter.Substring(1);
            }

            var _encode = _parameterEncodeChecker.CheckEncodeParameter(ref parameter, encode);

            if (requestParams != null)
            {
                if (parameter == "")
                {
                    value = requestParams.AllValues();   
                }
                else
                {
                    if (!requestParams.HasValue(parameter))
                    {
                        switch(paramMissingBehaviour)
                        {
                            case Definitions.ParamMissingBehaviour.ThrowException:
                                throw new Exception($"Requested parameter '{parameter}' not available.");

                            case Definitions.ParamMissingBehaviour.Blank:
                                value = "";
                                break;

                            default:
                                throw new Exception($"Unhandled behaviour '{paramMissingBehaviour}'");
                        }
                        if (paramMissingBehaviour == Definitions.ParamMissingBehaviour.ThrowException) 

                        value = "";
                    }
                    else
                    {

                        var result = requestParams.Value<object>(parameter);
                        value = (result == null)
                            ? ""
                            : result.ToString();
                    }
                }
            }
            else
            {
                value = _contextAccessor.HttpContext.Request.Method.ToUpperInvariant() == "POST"
                    ? _contextAccessor.HttpContext.Request.Form[parameter].ToString()
                    : (parameter == ""
                        ? _contextAccessor.HttpContext.Request.QueryString.Value
                        : _contextAccessor.HttpContext.Request.Query[parameter].ToString());
            }

            if (_encode) value = _valueEncoder.Encode(value, encodeType);

            output = output.Replace($"[?{(prefix ? "." : string.Empty)}{parameter}]", value);
            return output;
        }
    }
}