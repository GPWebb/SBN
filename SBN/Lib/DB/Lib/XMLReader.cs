using SBN.Lib.Sys;
using System;
using System.Data;
using System.Text;
using System.Xml.Linq;

namespace SBN.Lib.DB
{
    public class XMLReader : IXMLReader
    {
        private readonly IXmlTryReader _xmlTryReader;

        public XMLReader(IXmlTryReader xmlTryReader)
        {
            _xmlTryReader = xmlTryReader;
        }

        public XElement ReadFromDataTable(DataTable dataTable)
        {
            var result = TryReadFromDataTable(dataTable, out var document);
            if (result) return document;

            throw new Exception("Input data cannot be parsed as XML");
        }

         public bool TryReadFromDataTable(DataTable dataTable, out XElement document)
         {
            StringBuilder xmlString = new StringBuilder();

            //XML documents > 2033 characters get split across data rows?!
            for (var i = 0; i < dataTable.Rows.Count; i++)
            {
                xmlString.Append(dataTable.Rows[i][0].ToString());
            }
            return _xmlTryReader.TryRead(xmlString.ToString(), out document);
        }
    }
}
