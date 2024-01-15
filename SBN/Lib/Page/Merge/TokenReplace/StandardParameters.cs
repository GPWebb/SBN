using System;
using System.Threading.Tasks;

namespace SBN.Lib.Page.Merge.TokenReplace
{
    public class StandardParameters : IStandardParameters
    {
        private readonly IParameterEncodeChecker _parameterEncodeChecker;
        private readonly ISettings _settings;
        private readonly IValueEncoder _valueEncoder;
        private readonly ISettingCacher _settingCacher;

        public StandardParameters(IParameterEncodeChecker parameterEncodeChecker,
            ISettings settings,
            IValueEncoder valueEncoder,
            ISettingCacher settingCacher)
        {
            _parameterEncodeChecker = parameterEncodeChecker;
            _settings = settings;
            _valueEncoder = valueEncoder;
            _settingCacher = settingCacher;
        }

        public async Task<string> StdParams(string input,
            Definitions.ParamEncode encode,
            Definitions.EncodeType encodeType,
            string pageTitle,
            Guid sessionToken,
            bool passThrough = false)
        {
            string output = input;

            var position = 0;

            while (position < output.Length)
            {
                string value;
                position = output.IndexOf("[!", position);
                if (position < 0)
                {
                    position = output.Length;
                }
                else
                {
                    var parameter = output.Substring(position + 2, output.IndexOf("]", position) - position - 2);
                    var originalParameter = parameter;

                    try
                    {
                        var _encode = _parameterEncodeChecker.CheckEncodeParameter(ref parameter, encode);

                        if (parameter.ToUpper() == "PAGETITLE")
                        {
                            value = pageTitle;
                        }
                        else if (_settingCacher.TryRead(sessionToken, parameter, out var cachedSetting))
                        {
                            value = cachedSetting;
                        }
                        else
                        {
                            value = await _settings.GetSetting(parameter);
                        }

                        if (_encode) value = _valueEncoder.Encode(value, encodeType);

                        output = output.Replace($"[!{originalParameter}]", value);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Error retrieving standard parameter: {ex.Message}", ex);
                    }
                    position++;
                }
            }

            return output;
        }
    }
}