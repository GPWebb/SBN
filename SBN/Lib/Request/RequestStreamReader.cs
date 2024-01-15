using System.IO;
using System.Text;
using System.Xml.Linq;

namespace SBN.Lib.Request
{
    public class RequestStreamReader : IRequestStreamReader
    {
        const int bodyReadBufferSize = 1024;
        const bool detectEncodingFromByteOrderMarks = true;
        const bool leaveOpen = false;

        public XDocument Read(Stream requestStream)
        {
            using (var reader = new StreamReader(requestStream,
                Encoding.UTF8,
                detectEncodingFromByteOrderMarks,
                bodyReadBufferSize,
                leaveOpen))
            {
                var requestBodyStr = reader.ReadToEnd();

                return string.IsNullOrWhiteSpace(requestBodyStr)
                    ? null
                    : XDocument.Parse(requestBodyStr);
            }
        }
    }
}
