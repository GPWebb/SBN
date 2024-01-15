using System;

namespace SBN.Lib.Page.Merge
{
    public class DataMergeParameterParser : IDataMergeParameterParser
    {
        public DataMergeParameter Parse(string parameter)
        {
            int? ActionID = null;
            string Parameter;
            string XPathParameter;

            if (parameter.Contains("|"))
            {
                var parts = parameter.Split('|');
                try
                {
                    ActionID = int.Parse(parts[0]);
                    Parameter = parts[1];
                }
                catch (Exception)
                {
                    throw new Exception($"Expected to find a parameter in the format <<ActionID>>|NamedParameter, found '{parameter}'");
                }
            }
            else
            {
                Parameter = parameter;
            }

            var parameterXML = Parameter.Replace(" ", "_x0020_");

            XPathParameter = parameterXML.Contains("/")
                ? parameterXML
                : $"//{parameterXML}";

            return new DataMergeParameter
            {
                ActionID = ActionID,
                Parameter = Parameter,
                XPathParameter = XPathParameter
            };
        }
    }
}
