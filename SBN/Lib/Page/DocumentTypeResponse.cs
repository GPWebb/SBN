using System.Net;

namespace SBN.Lib.Page
{
    public class DocumentTypeResponse
    {
        public int DocumentTypeID { get; set; }
        public string Query { get; set; }
        public HttpStatusCode ResponseCode { get; set; }
    }
}
