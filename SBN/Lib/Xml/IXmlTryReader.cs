using System;
using System.Xml.Linq;

namespace SBN.Lib.Sys
{
    public interface IXmlTryReader
    {
        bool TryRead(string input, out XElement document);
        bool TryRead(string input, out XElement document, out Exception exception);
    }
}