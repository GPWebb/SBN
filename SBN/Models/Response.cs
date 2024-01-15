using System.Net;
using System.Xml.Linq;
using SBN.Lib.Definitions;

namespace SBN.Models
{
    public class Response
    {
        public string Url { get; set; }

        public string Message { get; set; }

        public OutcomeMessageType? MessageType { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public XElement Data { get; set; }

        public XElement OutputParameters { get; set; }

        public string APIRoute { get; set; }
    }
}
