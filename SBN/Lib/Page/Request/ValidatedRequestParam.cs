using System;

namespace SBN.Lib.Page.Request
{
    public class ValidatedRequestParam
    {
        #region "Properties"
        public string PName { get; set; }
        public Type PType { get; set; }
        public decimal? Min { get; set; }
        public decimal? Max { get; set; }
        public object DefaultValue { get; set; }
        public bool Required { get; set; }
        public object Value { get; set; }
        #endregion

        #region "Constructors. Add more as required"
        public ValidatedRequestParam(string name, Type type, bool required)
        {
            PName = name;
            PType = type;
            Required = required;
        }
        public ValidatedRequestParam(string name, Type type, object defaultValue, bool required)
        {
            PName = name;
            PType = type;
            DefaultValue = defaultValue;
            Required = required;
        }

        public ValidatedRequestParam(string name, Type type, decimal? min, decimal? max, decimal? defaultValue, bool required)
        {
            PName = name;
            PType = type;
            Min = min;
            Max = max;
            DefaultValue = defaultValue;
            Required = required;
        }

        public ValidatedRequestParam(string name, string type, string defaultValue, decimal? min, decimal? max, bool required, string value)
        {
            PName = name;
            PType = TypeFromName(type);
            Min = min;
            Max = max;
            Required = required;
            DefaultValue = PopulateValue(type, defaultValue);
            Value = PopulateValue(type, value);
        }

        private object PopulateValue(string type, string defaultValue)
        {
            if (defaultValue == null) return null;

            switch (type.ToLowerInvariant())
            {
                case "string":
                    return defaultValue;

                case "integer":
                case "int":
                    if (int.TryParse(defaultValue, out var dfi)) return dfi;

                    throw new ArgumentException($"Invalid integer parameter '{defaultValue}'");

                case "decimal":
                    if (decimal.TryParse(defaultValue, out var dfd)) return dfd;

                    throw new ArgumentException($"Invalid decimal parameter '{defaultValue}'");

                default:
                    throw new ArgumentException($"Unrecognised type '{type}' in parameter declaration");
            }
        }

        private Type TypeFromName(string type)
        {
            switch (type.ToLowerInvariant())
            {
                case "string":
                    return typeof(string);

                case "integer":
                case "int":
                    return typeof(int);

                case "decimal":
                    return typeof(decimal);

                default:
                    throw new ArgumentException($"Unrecognised type '{type}' in parameter declaration");
            }
        }
        #endregion
    }
}
