using System.IO;
using System.Xml.Linq;

namespace SBN.Lib.Request
{
    public interface IRequestStreamReader
    {
        XDocument Read(Stream requestStream);
    }
}