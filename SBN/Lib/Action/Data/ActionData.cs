using System;
using System.Data;
using System.Net;
using System.Xml.Linq;
using SBN.Lib.Action.Outcome;

namespace SBN.Lib.Action.Data
{
    public class ActionData
    {
        public int ActionID { get; set; }
        public string ActionParameters { get; set; }
        public string SourceURL { get; set; }
        public DataSet Data { get; set; }
        public XElement Definition { get; set; }
        public HttpStatusCode? StatusCode { get; set; }
        public ApiActionOutcome Outcome { get; set; }
        public XElement Transform { get; set; }
        
        public DateTime? CacheExpiry { get; set; }
        public bool? CacheBySession { get; set; }
    }
}
