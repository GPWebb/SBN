using System.Data;
using System.Xml.Linq;
using SBN.Lib.DB.Lib;

namespace SBN.Lib.Page
{
    public class PagePart
    {
        public PagePart(DataRow pagePart)
        {
            PartXML = pagePart.XmlField("PartXML");
            PartXML_ActionID = pagePart.Field<int?>("PartXML_ActionID");
            Source_ActionID = pagePart.Field<int?>("Source_ActionID");
            ActionParameters = pagePart.Field<string>("ActionParameters");
            DelayLoad = pagePart.Field<bool?>("DelayLoad");
            PartPath = pagePart.Field<string>("PartPath");
            Transform = pagePart.XmlField("TransformXsl");
            Position = pagePart.Field<int>("Position");
            Reference = pagePart.Field<string>("Reference");
        }

        public XElement PartXML { get; set; }
        public int? PartXML_ActionID { get; set; }
        public int? Source_ActionID { get; set; }
        public string ActionParameters { get; set; }
        public bool? DelayLoad { get; set; }

        public string PartPath { get; set; }
        public XElement Transform { get; set; }
        public int Position { get; set; }
        public string Reference { get; set; }
    }
}
