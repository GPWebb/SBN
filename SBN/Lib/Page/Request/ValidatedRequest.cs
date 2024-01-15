using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace SBN.Lib.Page.Request
{
    public class ValidatedRequest
    {
        private bool _usePost;
        private List<ValidatedRequestParam> _params = new List<ValidatedRequestParam>();
        private IEnumerable<KeyValuePair<string, string>> _values;

        public int Params() => _params.Count();
        public IEnumerable<string> AllParams => _params.Select(x => x.PName);

        public string ErrorText { get; private set; }

        public ValidatedRequest(IEnumerable<ValidatedRequestParam> validatedRequestParams, HttpContext context, bool usePost)
        {
            _params = validatedRequestParams.ToList();

            _values = _usePost
                ? context.Request.Form.Select(x => new KeyValuePair<string, string>(x.Key, x.Value))
                : context.Request.Query.Select(x => new KeyValuePair<string, string>(x.Key, x.Value));

            _usePost = usePost;
        }

        public bool Valid()
        {
            StringBuilder error = new StringBuilder();

            foreach (ValidatedRequestParam p in _params)
            {
                if (p.Required & !_values.Any(v => v.Key == p.PName)) error.AppendLine($"Required parameter {p.PName} missing");
                else
                {
                    string _type = p.PType.FullName;

                    if (_type == "System.Int32" | _type == "System.Decimal")
                    {
                        if (p.Min != null & p.Min > Convert.ToDecimal(_values.Single(v => v.Key == p.PName)) |
                            (p.Max != null & p.Max < Convert.ToDecimal(_values.Single(v => v.Key == p.PName))))
                        {
                            error.AppendLine($"Parameter {p.PName} has value outside the specified range");
                        }
                    }
                }
                //TODO Add type checking here too? Not fast.
            }

            ErrorText = error.ToString();

            return ErrorText == string.Empty;
        }

        public T Value<T>(string name)
        {
            ValidatedRequestParam pm = _params.Where(p => p.PName == name).FirstOrDefault();

            if (pm == null) throw new Exception($"Requested parameter {name} not defined. Check your parameter definitions");

            if (pm.PType != typeof(T) && typeof(T) != typeof(object))
                throw new Exception("Requested type does not match the parameter's type");

            if (pm.Value != null) return (T)Convert.ChangeType(pm.Value, pm.PType);

            if (!_values.Any(x => x.Key == name) || _values.Single(x => x.Key == name).Value == string.Empty)
            {
                if (pm.DefaultValue != null) return (T)Convert.ChangeType(pm.DefaultValue, pm.PType);

                if (pm.Required) throw new Exception($"Required parameter {name} missing");

                return default;
            }

            return (T)Convert.ChangeType(_values.Single(x => x.Key == name).Value, pm.PType);
        }

        public string AllValues()
        {
            if (_params.Count == 0) return "";

            return "?" + string.Join("&", _params.Select(x => $"{HttpUtility.UrlEncode(x.PName)}={HttpUtility.UrlEncode(x.Value?.ToString()??"")}"));
        }

        internal void SetParam(ValidatedRequestParam param)
        {
            _params.Add(param);
        }

        public bool HasValue(string name)
        {
            return _params.Exists(p => p.PName == name);
        }
    }
}
