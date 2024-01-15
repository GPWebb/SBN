using System;
using System.Xml.Linq;

namespace SBN.Lib.Sys
{
    public class XmlTryReader : IXmlTryReader
    {
        public bool TryRead(string input, out XElement document)
        {
            try
            {
                document = XElement.Parse(input);
                return true;
            }
            catch
            {
                document = null;
                return false;
            }
        }

        public bool TryRead(string input, out XElement document, out Exception exception)
        {
            try
            {
                document = XElement.Parse(input);
                exception = null;
                return true;
            }
            catch(Exception ex)
            {
                document = null;
                exception = ex;
                return false;
            }
        }

    }
}
