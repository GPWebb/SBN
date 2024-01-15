using System.Xml.Linq;

namespace SBN.Lib.Page
{
    public class PageDocument
    {
        public int DocumentTypeID { get; set; }
        public XElement Document { get; set; }
        public XElement TransformXsl { get; set; }
        public int Position { get; set; }
        public string PartPath { get; set; }
    }
}
